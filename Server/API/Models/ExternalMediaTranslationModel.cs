using System.ComponentModel.DataAnnotations;

namespace Server.API.Models;

public sealed class ExternalMediaTranslationModel
{
    public Guid ExternalMediaId { get; set; }
    public required ExternalMediaModel ExternalMedia { get; set; }
    
    [MaxLength(2)]
    public required string LanguageCode { get; set; }
    public required string Text { get; set; }
}