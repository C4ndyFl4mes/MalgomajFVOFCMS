using FastEndpoints;
using FluentValidation;
using Server.API.Enums;

namespace Server.API.Routes.ImageFile.GET;

public record GetImagesRequest
{
    public required string[] Types { get; set; } // What images of ImageTypes should be fetched.
}

public record ImageDTO
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text.
}

public record GetImagesResponse
{
    public required IEnumerable<ImageDTO> Images { get; set; }
}

public class GetImagesRequestValidator : Validator<GetImagesRequest>
{
    public GetImagesRequestValidator()
    {
        RuleFor(x => x.Types)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Bildtyper måste anges.")
            .NotEmpty().WithMessage("Bildtyper måste anges.");

        RuleForEach(x => x.Types)
            .Must(type =>
                !string.IsNullOrWhiteSpace(type) &&
                Enum.TryParse<ImageType>(type, true, out var parsed) &&
                Enum.IsDefined(parsed))
            .WithMessage("En eller flera av de valda bildtyperna är felaktiga.");
    }
}