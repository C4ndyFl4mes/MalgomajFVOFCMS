using Microsoft.AspNetCore.Components;
using Server.API.Routes.ImageFile.POST;
using Server.UI.Layout;
using Server.API.Enums;
using Microsoft.AspNetCore.Components.Forms;
using FluentValidation;
using FluentValidation.Results;
using Server.API.Exceptions;

namespace Server.UI.Pages.FilesPage.ImagesPage;

public class ImagesBase : ComponentBase
{
    [Inject]
    protected NavigationState NavigationState { get; set; } = default!;
    [Inject]
    protected IValidator<PostImageRequest> PostImageValidator { get; set; } = default!;
    [Inject]
    protected ImagePostData ImagePostData { get; set; } = default!;

    protected Dictionary<string, string[]> ValidationErrors { get; set; } = [];
    protected (bool IsSuccess, string Message) ResultMessage { get; set; } = (false, string.Empty);
    protected bool IsNewImageOverlayOpen { get; set; } = false;
    
    protected IBrowserFile? NewImageFile { get; set; } = null;
    protected ImageType NewImageType { get; set; } = ImageType.Normal;
    protected List<(string, ImageType)> ImageTypes { get; set; } = new()
    {
        ("Normal (16:9)", ImageType.Normal),
        ("Banderoll (3:1)", ImageType.Banner),
        ("Fyrkant (1:1)", ImageType.Square),
        ("Ikon (1:1)", ImageType.Icon)
    };

    protected Dictionary<string, string> NewImageTranslations
    {
        get => _newImageTranslations;
        set
        {
            _newImageTranslations = value;
            Console.WriteLine("NewImageTranslations updated:");
            foreach (var kvp in _newImageTranslations)            {
                Console.WriteLine($"LanguageCode: {kvp.Key}, Text: {kvp.Value}");
            }
        }
    }

    private Dictionary<string, string> _newImageTranslations = new() { ["sv"] = string.Empty };

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
                Title = "Filer",
                Href = "/admin/files"
            },
            new BreadcrumbModel
            {
                Title = "Bildhantering",
                Href = "/admin/files/images"
            }
        ]);
    }

    protected async Task HandleAddNewImageAsync()
    {
        if (NewImageFile is not null)
        {
            ValidationErrors.Clear();
            PostImageRequest request = new()
            {
                ImageFile = NewImageFile,
                Type = NewImageType.ToString(),
                Translations = NewImageTranslations
            };

            ValidationResult validationResult = await PostImageValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                ErrorsToDictionary(validationResult);
                await InvokeAsync(StateHasChanged);
                return;
            }

            try
            {
                await ImagePostData.PostImageAsync(request, CancellationToken.None);
                ResultMessage = (true, "Bilden har laddats upp.");
            }
            catch (BadRequestException ex)
            {
                ResultMessage = (false, ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ResultMessage = (false, ex.Message);
            }
            catch (Exception)
            {
                ResultMessage = (false, "Ett oväntat fel inträffade. Vänligen försök igen senare.");
            }

            await InvokeAsync(StateHasChanged);
        }
    }

    protected void ResetNewImageForm()
    {
        NewImageFile = null;
        NewImageType = ImageType.Normal;
        NewImageTranslations = new Dictionary<string, string> { ["sv"] = string.Empty };
        ValidationErrors.Clear();
        ResultMessage = (false, string.Empty);
    }

    protected void SetImageField(InputFileChangeEventArgs e)
    {
        IsNewImageOverlayOpen = true;
        NewImageFile = e.File;
        StateHasChanged();
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