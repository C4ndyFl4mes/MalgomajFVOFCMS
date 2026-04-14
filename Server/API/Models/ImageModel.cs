using Server.API.Enums;

namespace Server.API.Models;

public sealed class ImageModel
{
    public Guid Id { get; set; }
    public required ImageType Type { get; set; }

    // Navigation property for translations
    public required ICollection<ImageAltTranslationModel> Translations { get; set; }

    // Optional one-to-one relationship: An image may be a slide, but that is not required.
    public SlideModel? Slide { get; set; }
}