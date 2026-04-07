using System.ComponentModel.DataAnnotations;

namespace Server.API.Models;

public sealed class ImageAltTranslationModel
{
    public Guid ImageId { get; set; }
    public required ImageModel Image { get; set; }
    
    [MaxLength(2)]
    public required string LanguageCode { get; set; }
    public required string Alt { get; set; }
}