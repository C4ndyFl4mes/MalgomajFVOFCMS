using Microsoft.AspNetCore.Components;
using Server.UI.Layout;

namespace Server.UI.Pages.ProfilePage;

public class ProfileBase : ComponentBase
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
                Title = "Profil",
                Href = "/admin/profile"
            }
        ]);
    }
}
