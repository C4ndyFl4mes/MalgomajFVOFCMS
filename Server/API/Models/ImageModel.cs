namespace Server.API.Models;

public sealed class ImageModel
{
    public Guid Id { get; set; }

    // Navigation property for translations
    public required ICollection<ImageAltTranslationModel> Translations { get; set; }
}