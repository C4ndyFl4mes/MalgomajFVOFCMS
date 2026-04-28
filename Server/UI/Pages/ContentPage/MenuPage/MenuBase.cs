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
                Href = "/"
            },
            new BreadcrumbModel
            {
                Title = "Innehåll",
                Href = "/content"
            },
            new BreadcrumbModel
            {
                Title = "Meny",
                Href = "/content/menu"
            }
        ]);
    }
}
