using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Menu.POST;

public class PostMenuData(AppDbContext ctx)
{
    public async Task PostMenuAsync(PostMenuRequest request, CancellationToken ct)
    {
        await using var transaction = await ctx.Database.BeginTransactionAsync(ct);

        List<ImageModel> icons = await ctx.Images.Where(image => image.Type == Enums.ImageType.Icon).ToListAsync(ct);
        List<PageModel> pages = await ctx.Pages.Where(page => page.IsPublished) .ToListAsync(ct);

        // Check all pageIds in request exist in the database, including children:
        List<Guid> pageIdsFromRequest = request.MenuItems.Select(mi => mi.PageId).ToList();
        foreach (var parentItem in request.MenuItems)
        {
            if (parentItem.Children != null)
            {
                pageIdsFromRequest.AddRange(parentItem.Children.Select(child => child.PageId));
            }
        }
        if (!pageIdsFromRequest.All(id => pages.Any(page => page.Id == id)))
        {
            throw new KeyNotFoundException("Hittade inte en eller flera av sidorna som menyalternativen leder till.");
        }

        // Check all ids are unique from request, including children:
        List<Guid> menuItemIds = request.MenuItems.Select(mi => mi.MenuItemId).ToList();
        foreach (var parentItem in request.MenuItems)
        {
            if (parentItem.Children != null)
            {
                menuItemIds.AddRange(parentItem.Children.Select(child => child.MenuItemId));
            }
        }
        if (menuItemIds.Distinct().Count() != menuItemIds.Count)
        {
            throw new ArgumentException("Alla menyalternativ måste ha unika id:n.");
        }

        // Check all menu items leads to unique pages, including children:
        List<Guid> pageIds = request.MenuItems.Select(mi => mi.PageId).ToList();
        foreach (var parentItem in request.MenuItems)
        {
            if (parentItem.Children != null)
            {
                pageIds.AddRange(parentItem.Children.Select(child => child.PageId));
            }
        }
        if (pageIds.Distinct().Count() != pageIds.Count)
        {
            throw new ArgumentException("Alla menyalternativ måste leda till unika sidor.");
        }

        List<MenuItemModel> menuItems = await ClearAsync(ct);

        foreach (PostMenuItemDTO item in request.MenuItems)
        {
            ImageModel icon = GetIcon(icons, item.IconId);
            menuItems.Add(new MenuItemModel
            {
                Id = item.MenuItemId,
                PageId = item.PageId,
                ParentId = null,
                IconId = icon.Id,
                Icon = icon,
                SortOrder = item.SortOrder,
                CustomUrl = item.CustomUrl
            });
        }

        // Set parentId for children:
        foreach (PostMenuItemDTO parentItem in request.MenuItems)
        {
            if (parentItem.Children != null)
            {
                foreach (PostMenuItemDTO childItem in parentItem.Children)
                {
                    ImageModel childIcon = GetIcon(icons, childItem.IconId);

                    // A new MenuItemModel for the child is created and added to the menuItems list:
                    MenuItemModel childMenuItem = new MenuItemModel
                    {
                        Id = childItem.MenuItemId,
                        PageId = childItem.PageId,
                        ParentId = parentItem.MenuItemId, // Set parentId to the parent's MenuItemId
                        IconId = childIcon.Id,
                        Icon = childIcon,
                        SortOrder = childItem.SortOrder,
                        CustomUrl = childItem.CustomUrl
                    };
                    menuItems.Add(childMenuItem);
                }
            }
        }


        await ctx.MenuItems.AddRangeAsync(menuItems, ct);

        await ctx.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    private async Task<List<MenuItemModel>> ClearAsync(CancellationToken ct)
    {
        List<MenuItemModel> existingMenuItems = await ctx.MenuItems.ToListAsync(ct);
        ctx.MenuItems.RemoveRange(existingMenuItems);
        existingMenuItems.Clear();

        return existingMenuItems;
    }

    private static ImageModel GetIcon(List<ImageModel> icons, Guid? incomingId)
    {
        ImageModel? icon = icons.FirstOrDefault(icon => icon.Id == incomingId);
        if (icon == null)
        {
            throw new KeyNotFoundException("Hittade inte ikonen till en eller flera av menyalternativen.");
        }

        return icon;
    }
}