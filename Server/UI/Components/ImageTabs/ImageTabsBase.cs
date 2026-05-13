using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Server.API.Enums;
using Server.API.Routes.ImageFile.GET;

namespace Server.UI.Components.ImageTabs;

public class ImageTabsBase : ComponentBase
{
    [Inject]
    protected IValidator<GetImagesRequest> GetImagesValidator { get; set; } = default!;
    [Inject]
    protected GetImagesData GetImagesData { get; set; } = default!;

    [Parameter]
    public required EventCallback<(ImageType, IEnumerable<ImageDTO>)> OnTabSelected { get; set; }
    [Parameter]
    public required EventCallback<Dictionary<string, string[]>> OnValidationErrors { get; set; }
    [Parameter]
    public required List<ImageType> AllowedImageTabs { get; set; }

    protected ImageType CurrentTab { get; set; } = ImageType.Normal;

    // De klickbara tabbarna och deras tillhörande bildtyper.
    protected Dictionary<ImageType, string>? ImageTabTypes { get; set; }

    // De bilder som visas i den aktiva tabben.
    protected Dictionary<ImageType, IEnumerable<ImageDTO>>? ActiveTabImages { get; set; }

    protected Dictionary<string, string[]> ValidationErrors { get; set; } = [];

    // Hanterar när en tab klickas på: uppdaterar den aktiva tabben, laddar bilderna för den tabben och skickar dem till föräldrakomponenten.
    protected async Task SelectTab(ImageType type)
    {
        if (ActiveTabImages is null)
            return;
        CurrentTab = type;
        ActiveTabImages[type] = await GetImagesAsync(type);
        await OnTabSelected.InvokeAsync((type, ActiveTabImages[type]));
    }

    // Ladda om bilderna i den aktiva tabben när en ny bild har laddats upp.
    public async Task ReloadActiveTab()
    {
        if (ActiveTabImages?.ContainsKey(CurrentTab) == true)
            await SelectTab(CurrentTab);
    }

    protected override async Task OnInitializedAsync()
    {
        Dictionary<ImageType, string> imageTabs = new()
        {
            [ImageType.Normal] = "Normal (16:9)",
            [ImageType.Banner] = "Banderoll (3:1)",
            [ImageType.Square] = "Fyrkant (1:1)",
            [ImageType.Icon] = "Ikon (1:1)"
        };

        Dictionary<ImageType, IEnumerable<ImageDTO>> activeTabs = new()
        {
            [ImageType.Normal] = [],
            [ImageType.Banner] = [],
            [ImageType.Square] = [],
            [ImageType.Icon] = []
        };
        
        HashSet<ImageType> allowed = AllowedImageTabs.ToHashSet();

        ImageTabTypes = imageTabs
            .Where(kvp => allowed.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        ActiveTabImages = activeTabs
            .Where(kvp => allowed.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        if (AllowedImageTabs.Count >= 1)
            await SelectTab(AllowedImageTabs[0]);
    }

    // Hämtar bilder av en viss typ från servern och hanterar eventuella valideringsfel eller andra fel som kan uppstå.
    protected async Task<IEnumerable<ImageDTO>> GetImagesAsync(ImageType type)
    {
        GetImagesRequest request = new()
        {
            Types = [type.ToString()]
        };

        ValidationResult validationResult = await GetImagesValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            ErrorsToDictionary(validationResult);
            await OnValidationErrors.InvokeAsync(ValidationErrors);
            return [];
        }

        try
        {
            GetImagesResponse response = await GetImagesData.GetAllImagesAsync(request.Types, CancellationToken.None);
            return response.Images;
        }
        catch (DirectoryNotFoundException ex)
        {
            ValidationErrors = new Dictionary<string, string[]>
            {
                ["Images"] = new[] { ex.Message }
            };
            await OnValidationErrors.InvokeAsync(ValidationErrors);
            return [];
        }
        catch (KeyNotFoundException ex)
        {
            ValidationErrors = new Dictionary<string, string[]>
            {
                ["Images"] = new[] { ex.Message }
            };
            await OnValidationErrors.InvokeAsync(ValidationErrors);
            return [];
        }
        catch (Exception ex)
        {
            ValidationErrors = new Dictionary<string, string[]>
            {
                ["Images"] = [$"Ett fel inträffade: {ex.Message}"]
            };
            await OnValidationErrors.InvokeAsync(ValidationErrors);
            return [];
        }
    }

    private void ErrorsToDictionary(ValidationResult validationResult)
    {
        ValidationErrors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
    }
}