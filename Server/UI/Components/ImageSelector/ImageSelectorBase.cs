using Microsoft.AspNetCore.Components;
using Server.API.Enums;
using Server.API.Routes.ImageFile.GET;

namespace Server.UI.Components.ImageSelector;

public class ImageSelectorBase : ComponentBase
{
    [Parameter]
    public EventCallback<ImageInspectionModel> OnImageSelected { get; set; }

    protected (ImageType, IEnumerable<ImageDTO>) ImagesOfSelectedTab { get; set; } = (ImageType.Normal, []);
    protected Dictionary<string, string[]> ValidationErrorsOfImageRetrieval { get; set; } = [];

    protected async Task SelectImage(ImageDTO image, int width, int height)
    {
        ImageInspectionModel inspectionModel = new()
        {
            Image = image,
            Width = width,
            Height = height
        };
        await OnImageSelected.InvokeAsync(inspectionModel);
    }

    protected override async Task OnInitializedAsync()
    {

    }

    // CSS-klasser för att bibehålla rätt bildförhållande i de olika tabbarna.
    protected Dictionary<ImageType, string> CSSClassesForImageType = new()
    {
        [ImageType.Normal] = "aspect-video",
        [ImageType.Banner] = "aspect-[3/1]",
        [ImageType.Square] = "aspect-square",
        [ImageType.Icon] = "aspect-square"
    };

    // Dimensioner för de olika bildtyperna, används i attributen.
    protected Dictionary<ImageType, (int, int)> DimensionsForImageType = new()
    {
        [ImageType.Normal] = (800, 450),
        [ImageType.Banner] = (1200, 400),
        [ImageType.Square] = (380, 380),
        [ImageType.Icon] = (64, 64)
    };
}