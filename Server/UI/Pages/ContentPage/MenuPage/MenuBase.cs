using Microsoft.AspNetCore.Components;
using Server.UI.Layout;

namespace Server.UI.Pages.ContentPage.MenuPage;

public class MenuBase : ComponentBase
{
    [Inject]
    protected NavigationState NavigationState { get; set; } = default!;

    protected override void OnInitialized()
    {
        NavigationState.SetBreadcrumbs([
            new BreadcrumbModel
            {
                Title = "Panel",
                Href = "/admin"
            },
            new BreadcrumbModel
            {
                Title = "Innehåll",
                Href = "/admin/content"
            },
            new BreadcrumbModel
            {
                Title = "Meny",
                Href = "/admin/content/menu"
            }
        ]);
    }
}
