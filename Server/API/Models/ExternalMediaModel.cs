using System.ComponentModel.DataAnnotations;
using Server.API.Enums;

namespace Server.API.Models;

public sealed class ExternalMediaModel
{
    public Guid Id { get; set; }
    public required string Url { get; set; }

    [EnumDataType(typeof(MediaType))]
    public required MediaType Type { get; set; }

    // Navigation property for translations
    public required ICollection<ExternalMediaTranslationModel> Translations { get; set; }
}