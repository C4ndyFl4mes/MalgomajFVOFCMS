using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Server.API.Data;
using Server.API.Exceptions;
using Server.API.Models;

namespace Server.API.Routes.Menu.POST;

public class PostMenuData(AppDbContext ctx)
{
    public async Task<PostMenuResponse> PostMenuAsync(PostMenuRequest request, CancellationToken ct)
    {
        await using IDbContextTransaction transaction = await ctx.Database.BeginTransactionAsync(ct);

        const int maxDepth = 3;

        List<ImageModel> icons = await ctx.Images.Where(image => image.Type == Enums.ImageType.Icon).ToListAsync(ct);

        HashSet<Guid> publishedPageIds = await ctx.Pages
            .Where(page => page.IsPublished)
            .Select(page => page.Id)
            .ToHashSetAsync(ct);

        List<(PostMenuItemDTO Item, Guid? ParentId, int Depth)> flatItems = Flatten(request.MenuItems).ToList();

        if (flatItems.Any(x => x.Depth > maxDepth))
            throw new BadRequestException($"Menyn får maximalt ha {maxDepth} nivåer.");
        
        List<Guid> pageIdsFromRequest = flatItems.Select(x => x.Item.PageId).ToList();
        if (!pageIdsFromRequest.All(publishedPageIds.Contains))
            throw new KeyNotFoundException("Hittade inte en eller flera av sidorna som menyalternativen leder till.");
        
        List<Guid> menuItemIds = flatItems.Select(x => x.Item.MenuItemId).ToList();
        if (menuItemIds.Distinct().Count() != menuItemIds.Count)
            throw new BadRequestException("Alla menyalternativ måste ha unika id:n.");
        
        if (pageIdsFromRequest.Distinct().Count() != pageIdsFromRequest.Count)
            throw new BadRequestException("Alla menyalternativ måste leda till unika sidor.");
        
        await ClearAsync(ct);

        List<MenuItemModel> menuItemsToInsert = flatItems.Select(x =>
        {
            ImageModel? icon = GetIcon(icons, x.Item.IconId);

            return new MenuItemModel
            {
                Id = x.Item.MenuItemId,
                PageId = x.Item.PageId,
                ParentId = x.ParentId,
                IconId = icon?.Id,
                Icon = icon,
                SortOrder = x.Item.SortOrder,
                CustomUrl = x.Item.CustomUrl
            };
        }).ToList();

        await ctx.MenuItems.AddRangeAsync(menuItemsToInsert, ct);

        await ctx.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return new PostMenuResponse
        {
            Message = "Lyckades spara huvudmenyn."
        };
    }

    private async Task<List<MenuItemModel>> ClearAsync(CancellationToken ct)
    {
        List<MenuItemModel> existingMenuItems = await ctx.MenuItems.ToListAsync(ct);
        ctx.MenuItems.RemoveRange(existingMenuItems);
        existingMenuItems.Clear();

        return existingMenuItems;
    }

    private static ImageModel? GetIcon(List<ImageModel> icons, Guid? incomingId)
    {
        ImageModel? icon = icons.FirstOrDefault(icon => icon.Id == incomingId);
        if (icon is null && incomingId is not null)
        {
            throw new KeyNotFoundException("Hittade inte ikonen till en eller flera av menyalternativen.");
        }

        return icon;
    }

    private static IEnumerable<(PostMenuItemDTO Item, Guid? ParentId, int Depth)> Flatten(
        IEnumerable<PostMenuItemDTO> items,
        Guid? parentId = null,
        int depth = 1)
    {
        foreach (PostMenuItemDTO item in items)
        {
            yield return (item, parentId, depth);

            // var = (PostMenuItemDTO Item, Guid? ParentId, int Depth)
            foreach (var child in Flatten(item.Children, item.MenuItemId, depth + 1))
            {
                yield return child;
            }
        }
    }
}