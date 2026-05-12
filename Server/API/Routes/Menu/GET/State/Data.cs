using AngleSharp.Common;
using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Enums;
using Server.API.Models;

namespace Server.API.Routes.Menu.GET.State;

public class GetMenuStateData(AppDbContext ctx)
{
    public async Task<GetMenuStateResponse> GetMenuStateAsync(CancellationToken ct)
    {
        List<PageModel> pages = await ctx.Pages
            .Include(page => page.Translations)
            .Where(page => page.IsPublished && page.Type == Enums.PageType.Page)
            .ToListAsync(ct);

        List<MenuItemModel> menuItems = await ctx.MenuItems.ToListAsync(ct);

        HashSet<Guid> iconIdsInMenu = menuItems
            .Where(x => x.IconId.HasValue)
            .Select(x => x.IconId!.Value)
            .ToHashSet();

        HashSet<Guid> existingIconIds = await ctx.Images
            .Where(x => x.Type == ImageType.Icon && iconIdsInMenu.Contains(x.Id))
            .Select(x => x.Id)
            .ToHashSetAsync();

        if (iconIdsInMenu.Any(x => !existingIconIds.Contains(x)))
            throw new KeyNotFoundException("Hittade inte ikonen till en av menyalternativen.");

        Dictionary<Guid, PageModel> pagesById = pages.ToDictionary(x => x.Id);

        List<MenuItemModel> rootItems = menuItems
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.SortOrder)
            .ToList();

        Dictionary<Guid, List<MenuItemModel>> childrenByParent = menuItems
            .Where(x => x.ParentId.HasValue)
            .GroupBy(x => x.ParentId!.Value)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(x => x.SortOrder).ToList()
            );

        List<MenuItemDTO> inMenu = BuildTree(rootItems, childrenByParent, pagesById);

        HashSet<Guid> pagesInMenu = Flatten(inMenu)
            .Select(x => x.PageInfo.PageId)
            .ToHashSet();

        List<SimplePageInformationDTO> notInMenu = pages
            .Where(x => !pagesInMenu.Contains(x.Id))
            .Select(x => new SimplePageInformationDTO
            {
                PageId = x.Id,
                Title = x.Translations.FirstOrDefault(t => t.LanguageCode == "sv")?.Title ?? string.Empty
            }).ToList();

        return new GetMenuStateResponse
        {
            NotInMenu = notInMenu,
            InMenu = inMenu
        };
    }

    private static IEnumerable<MenuItemDTO> Flatten(IEnumerable<MenuItemDTO> items)
    {
        foreach (MenuItemDTO item in items)
        {
            yield return item;

            foreach (MenuItemDTO child in Flatten(item.Children))
            {
                yield return child;
            }
        }
    }

    private static List<MenuItemDTO> BuildTree(
        List<MenuItemModel> items,
        IDictionary<Guid, List<MenuItemModel>> childrenByParent,
        IDictionary<Guid, PageModel> pagesById)
    {

        List<MenuItemDTO> result = [];

        foreach (MenuItemModel item in items)
        {
            if (!item.PageId.HasValue || !pagesById.TryGetValue(item.PageId.Value, out PageModel? page))
                continue; // Hoppar över menu items som länkar till opublicerade/försvunna sidor.

            List<MenuItemModel> children = childrenByParent.GetOrDefault(item.Id, []);
           
            MenuItemDTO menuItem = new()
            {
                MenuItemId = item.Id,
                IconId = item.IconId,
                SortOrder = item.SortOrder,
                CustomUrl = item.CustomUrl,
                PageInfo = new SimplePageInformationDTO
                {
                    PageId = page.Id,
                    Title = page.Translations.FirstOrDefault(t => t.LanguageCode == "sv")?.Title ?? string.Empty
                },
                Children = BuildTree(children, childrenByParent, pagesById)
            };

            result.Add(menuItem);
        }

        return result;
    }
}