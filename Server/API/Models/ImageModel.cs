using Server.API.Enums;

namespace Server.API.Models;

public sealed class ImageModel
{
    public Guid Id { get; set; }
    public required ImageType Type { get; set; }

    // Navigation property for translations
    public required ICollection<ImageAltTranslationModel> Translations { get; set; }

    // Navigation property for the Slide entity (one-to-one relationship)
    public SlideModel? Slide { get; set; }
}