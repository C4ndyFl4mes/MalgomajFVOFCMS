using Microsoft.AspNetCore.Components;
using Server.UI.Layout;

namespace Server.UI.Pages.ManagementPage;

public class ManagementBase : ComponentBase
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
                Title = "Hantering",
                Href = "/admin/management"
            }
        ]);
    }
}
