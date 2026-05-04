using Microsoft.AspNetCore.Components;
using Server.UI.Layout;

namespace Server.UI.Pages.ManagementPage.UsersPage;

public class UsersBase : ComponentBase
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
            },
            new BreadcrumbModel
            {
                Title = "Användare",
                Href = "/admin/management/users"
            }
        ]);
    }
}
