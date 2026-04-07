using System.ComponentModel.DataAnnotations;

namespace Server.API.Models;

public sealed class PageTranslationModel
{
    public Guid PageId { get; set; }
    public required PageModel Page { get; set; }
    
    [MaxLength(2)]
    public required string LanguageCode { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string Slug { get; set; }
    public string? Excerpt { get; set; }
    public required string MetaDescription { get; set; }
    public required string MetaKeywords { get; set; }
}