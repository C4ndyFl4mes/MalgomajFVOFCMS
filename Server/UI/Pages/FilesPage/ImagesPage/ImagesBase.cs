using Microsoft.AspNetCore.Components;
using Server.UI.Layout;

namespace Server.UI.Pages.FilesPage.ImagesPage;

public class ImagesBase : ComponentBase
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
                Title = "Filer",
                Href = "/files"
            },
            new BreadcrumbModel
            {
                Title = "Bilder",
                Href = "/files/images"
            }
        ]);
    }
}