using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Server.API.Enums;

namespace Server.API.Routes.ImageFile.POST;

public record PostImageRequest
{
    [EnumDataType(typeof(ImageType)), Required]
    public required string Type { get; set; }

    [Required]
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text.

    [Required]
    public required IFormFile ImageFile { get; set; }
}

public record PostImageResponse
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
}

public class PostImageRequestValidator : AbstractValidator<PostImageRequest>
{
    public PostImageRequestValidator()
    {
        RuleFor(x => x.Type)
            .Must(type => Enum.TryParse<ImageType>(type, true, out _))
            .WithMessage("Ogiltig bildtyp. Tillåtna värden är: banner, normal, square, icon.");

        RuleFor(x => x.Translations)
            .NotEmpty()
            .WithMessage("Minst en översättning krävs.")
            .Must(translations => translations.Keys.All(lang => !string.IsNullOrWhiteSpace(lang)))
            .WithMessage("Språkkoder får inte vara tomma eller innehålla endast blanksteg.")
            .Must(translations => translations.Values.All(text => !string.IsNullOrWhiteSpace(text)))
            .WithMessage("Översättningstexter får inte vara tomma eller innehålla endast blanksteg.");

        RuleFor(x => x.ImageFile)
            .NotNull()
            .WithMessage("Bildfil krävs.")
            .Must(file => file.ContentType.StartsWith("image/"))
            .WithMessage("Uppladdad fil måste vara en bild.");
        
        RuleFor(x => x)
            .Must(HasValidFileTypeForImageType)
            .WithMessage("Ikoner måste laddas upp som SVG. Övriga bildtyper får inte vara SVG.");
        
        RuleFor(x => x)
            .Must(AllowedFileTypes)
            .WithMessage("Endast JPEG, PNG, WebP och SVG är tillåtna filtyper.");
    }

    private static bool HasValidFileTypeForImageType(PostImageRequest request)
    {
        if (!Enum.TryParse(request.Type, true, out ImageType imageType))
            return false;
        
        string extension = Path.GetExtension(request.ImageFile.FileName).ToLowerInvariant();
        string contentType = request.ImageFile.ContentType.ToLowerInvariant();

        bool isSvg = extension == ".svg" || contentType == "image/svg+xml";

        if (imageType == ImageType.Icon)
            return isSvg;
        
        return !isSvg && contentType.StartsWith("image/");
    }

    private static bool AllowedFileTypes(PostImageRequest request)
    {
        string[] allowedTypes = { "image/jpeg", "image/png", "image/webp", "image/svg+xml" };
        return allowedTypes.Contains(request.ImageFile.ContentType.ToLowerInvariant());
    }
}