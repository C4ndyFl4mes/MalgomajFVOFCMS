using Microsoft.AspNetCore.Components;
using Server.UI.Layout;

namespace Server.UI.Pages.ContentPage.PagesPage;

public class PagesBase : ComponentBase
{
    [Inject]
    protected NavigationState NavigationState { get; set; } = default!;

    protected Guid Id { get; set; } = Guid.NewGuid();
    protected string Href { get; set; } = string.Empty; // Href för att navigera till editorn, kan sättas baserat på Id eller annan logik.

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
                Title = "Sidor",
                Href = "/admin/content/pages"
            }
        ]);

        Href = $"/admin/content/pages/edit/{Id}"; 
    }
}
