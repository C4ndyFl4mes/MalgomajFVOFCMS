using Microsoft.AspNetCore.Components;

namespace Server.UI.Layout.Menu;

public class MenuBase : ComponentBase
{
    protected List<MenuItemModel> MenuItems { get; set; } = [];

    protected override void OnInitialized()
    {
        MenuItems = [
            new MenuItemModel {
                Title = "Panelen",
                Href = "/",
                Icon = GetIcon("dashboard")
            },
            new MenuItemModel {
                Title = "Filer",
                Href = "/files",
                Icon = GetIcon("file"),
                SubMenuItems = [
                    new MenuItemModel {
                        Title = "Bilder",
                        Href = "/files/images",
                        Icon = GetIcon("images")
                    }
                ]
            },
            new MenuItemModel {
                Title = "Innehåll",
                Href = "/content",
                Icon = GetIcon("content"),
                SubMenuItems = [
                    new MenuItemModel {
                        Title = "Meny",
                        Href = "/content/menu",
                        Icon = GetIcon("menu")
                    },
                    new MenuItemModel {
                        Title = "Sidor",
                        Href = "/content/pages",
                        Icon = GetIcon("page")
                    },
                    new MenuItemModel {
                        Title = "Media",
                        Href = "/content/media",
                        Icon = GetIcon("media")
                    },
                    new MenuItemModel {
                        Title = "Bildspel",
                        Href = "/content/slideshow",
                        Icon = GetIcon("slideshow")
                    },
                    new MenuItemModel {
                        Title = "Kontakt",
                        Href = "/content/contact",
                        Icon = GetIcon("contact")
                    },
                    new MenuItemModel {
                        Title = "Styrelse",
                        Href = "/content/board",
                        Icon = GetIcon("board")
                    },
                ]
            },
            new MenuItemModel {
                Title = "Hantering",
                Href = "/management",
                Icon = GetIcon("management"),
                SubMenuItems = [
                    new MenuItemModel {
                        Title = "Användare",
                        Href = "/management/users",
                        Icon = GetIcon("users")
                    },
                ]
            },
            new MenuItemModel {
                Title = "Profil",
                Href = "/profile",
                Icon = GetIcon("profile")
            }
        ];
    }
    protected static string GetIcon(string icon) => $"icons/{icon}.svg";
}

