using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using FluentValidation;

namespace Server.API.Routes.ImageFile.PUT;

public record PutImageRequest
{
    [Required]
    public required Guid Id { get; set; }
    [Required]
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text.
}

public record PutImageResponse
{
    public required string Message { get; set; }
}

public class PutImageRequestValidator : Validator<PutImageRequest>
{
    public PutImageRequestValidator()
    {
        RuleFor(x => x.Translations)
            .NotEmpty()
            .WithMessage("Minst en översättning krävs.")
            .Must(translations => translations.Keys.All(lang => !string.IsNullOrWhiteSpace(lang)))
            .WithMessage("Språkkoder får inte vara tomma eller innehålla endast blanksteg.")
            .Must(translations => translations.Values.All(text => !string.IsNullOrWhiteSpace(text)))
            .WithMessage("Översättningstexter får inte vara tomma eller innehålla endast blanksteg.");
    }
}