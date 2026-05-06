using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Server.API.Routes.ImageFile.DELETE;
using Server.API.Routes.ImageFile.GET;
using Server.API.Routes.ImageFile.PUT;

namespace Server.UI.Components.InspectImage;

public class InspectImageBase : ComponentBase
{
    [Inject]
    protected ImagePutData ImagePutData { get; set; } = default!;
    [Inject]
    protected DeleteImageData DeleteImageData { get; set; } = default!;
    [Inject]
    protected IValidator<PutImageRequest> PutImageValidator { get; set; } = default!;

    [Parameter]
    public required ImageInspectionModel ImageInspection { get; set; }
    [Parameter]
    public required EventCallback<Guid> OnImageDeleted { get; set; }

    protected bool IsDeleteConfirmationOpen { get; set; } = false;

    protected Dictionary<string, string[]> ValidationErrors { get; set; } = [];
    protected (bool IsSuccess, string Message) ResultMessage { get; set; } = (false, string.Empty);

    // Sparar ändringar av bildens översättningar.
    protected async Task SaveChangesAsync()
    {
        PutImageRequest request = new PutImageRequest
        {
            Id = ImageInspection.Image.Id,
            Translations = ImageInspection.Image.Translations
        };

        ValidationResult validationResult = await PutImageValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            ErrorsToDictionary(validationResult);
            await InvokeAsync(StateHasChanged);
            return;
        }

        try
        {
            PutImageResponse response = await ImagePutData.UpdateImageAsync(request, CancellationToken.None);
            ResultMessage = (true, response.Message);
        }
        catch (KeyNotFoundException ex)
        {
            ResultMessage = (false, ex.Message);
        }
        catch (Exception ex)
        {
            ResultMessage = (false, $"Ett oväntat fel inträffade: {ex.Message}");
        }
        finally
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    // Raderar bilden och uppdaterar UI.
    protected async Task DeleteImageAsync()
    {
        try
        {
            await DeleteImageData.DeleteImageAsync(ImageInspection.Image.Id, CancellationToken.None);
            ResultMessage = (true, "Bilden har raderats.");
        }
        catch (KeyNotFoundException ex)
        {
            ResultMessage = (false, ex.Message);
        }
        catch (Exception ex)
        {
            ResultMessage = (false, $"Ett oväntat fel inträffade: {ex.Message}");
        }
        finally
        {
            await OnImageDeleted.InvokeAsync(ImageInspection.Image.Id);
            await InvokeAsync(StateHasChanged);
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