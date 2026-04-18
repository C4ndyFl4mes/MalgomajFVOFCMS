using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Menu.GET.State;

public class GetMenuStateData(AppDbContext ctx)
{
    public async Task<GetMenuStateResponse> GetMenuStateAsync(CancellationToken ct)
    {
        List<PageModel> pages = await ctx.Pages.Include(page => page.Translations).Where(page => page.IsPublished && page.Type == Enums.PageType.Page).ToListAsync(ct);
        List<MenuItemModel> menuItems = await ctx.MenuItems.ToListAsync(ct);
        List<ImageModel> icons = await ctx.Images.Where(image => image.Type == Enums.ImageType.Icon).ToListAsync(ct);

        List<MenuItemDTO> inMenu = [];
        List<SimplePageInformationDTO> notInMenu = [];

        foreach (MenuItemModel menuItem in menuItems)
        {
            if (pages.Any(page => page.Id == menuItem.PageId))
            {
                PageModel page = pages.FirstOrDefault(page => page.Id == menuItem.PageId) ??
                    throw new KeyNotFoundException("Hittade inte en eller flera sidor motsvarande menyn.");

                if (menuItem.IconId.HasValue && !icons.Any(icon => icon.Id == menuItem.IconId))
                    throw new KeyNotFoundException("Hittade inte ikonen till en av menyalternativen.");
                
                // The hierarchical menu tree:
                inMenu.Add(
                    new MenuItemDTO
                {
                    MenuItemId = menuItem.Id,
                    IconId = menuItem.IconId,
                    SortOrder = menuItem.SortOrder,
                    CustomUrl = menuItem.CustomUrl,
                    PageInfo = new SimplePageInformationDTO
                    {
                        PageId = page.Id,
                        Title = page.Translations.FirstOrDefault(t => t.LanguageCode == "sv")?.Title ?? string.Empty
                    },
                    Children = menuItems.Where(mi => mi.ParentId == menuItem.Id).OrderBy(child => child.SortOrder).Select(child => new MenuItemDTO
                    {
                        MenuItemId = child.Id,
                        IconId = child.IconId,
                        SortOrder = child.SortOrder,
                        CustomUrl = child.CustomUrl,
                        PageInfo = new SimplePageInformationDTO
                        {
                            PageId = page.Id,
                            Title = page.Translations.FirstOrDefault(t => t.LanguageCode == "sv")?.Title ?? string.Empty
                        }
                    }).ToList()
                });

                // Remove standalone children from the InMenu list:
                foreach (MenuItemModel child in menuItems.Where(mi => mi.ParentId == menuItem.Id))
                {
                    inMenu.RemoveAll(mi => mi.MenuItemId == child.Id);
                }

                pages.Remove(page);
            }
        }

        foreach (PageModel page in pages)
        {
            notInMenu.Add(new SimplePageInformationDTO
            {
                PageId = page.Id,
                Title = page.Translations.FirstOrDefault(t => t.LanguageCode == "sv")?.Title ?? string.Empty
            });
        }

        return new GetMenuStateResponse
        {
            NotInMenu = notInMenu,
            InMenu = inMenu
        };
    }
}