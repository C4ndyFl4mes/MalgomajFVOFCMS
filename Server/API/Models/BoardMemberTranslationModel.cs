using System.ComponentModel.DataAnnotations;

namespace Server.API.Models;

public sealed class BoardMemberTranslationModel
{
    public Guid BoardMemberId { get; set; }
    public required BoardMemberModel BoardMember { get; set; }
    
    [MaxLength(2)]
    public required string LanguageCode { get; set; }
    public required string Text { get; set; }
}