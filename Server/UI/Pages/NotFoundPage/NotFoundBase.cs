using Microsoft.AspNetCore.Components;
using Server.UI.Layout;

namespace Server.UI.Pages.NotFoundPage;

public class NotFoundBase : ComponentBase
{
    [Inject]
    NavigationState NavigationState { get; set; } = default!;

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
                Title = "Sidan kunde inte hittas",
                Href = "/admin/not-found"
            }
        ]);
    }
}