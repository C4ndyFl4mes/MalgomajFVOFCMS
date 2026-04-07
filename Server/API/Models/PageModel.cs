using System.ComponentModel.DataAnnotations;
using Server.API.Enums;

namespace Server.API.Models;

public sealed class PageModel
{
    public Guid Id { get; set; }
    [EnumDataType(typeof(PageType))]
    public required PageType Type { get; set; }
    public required bool IsPublished { get; set; }
    public required DateTime SavedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property for the page's thumbnail image.
    public ImageModel? Thumbnail { get; set; }
    public Guid? ThumbnailId { get; set; }

    // Navigation property for translations.
    public required ICollection<PageTranslationModel> Translations { get; set; }
}