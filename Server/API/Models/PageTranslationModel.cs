using System.ComponentModel.DataAnnotations;

namespace Server.API.Models;

// The DTO provides the default values if the client doesn't provide them. Still, the Model is strict.
public sealed class PageTranslationModel
{
    public Guid PageId { get; set; }
    public required PageModel Page { get; set; }
    
    [MaxLength(2)]
    public required string LanguageCode { get; set; }
    [MaxLength(150)]
    public required string Title { get; set; } 
    public required string Content { get; set; }
    public required string Slug { get; set; }
    [MaxLength(300)]
    public required string MetaDescription { get; set; }
    [MaxLength(300)]
    public required string MetaKeywords { get; set; }
}