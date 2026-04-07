namespace Server.API.Models;

public sealed class BoardMemberModel
{
    public Guid Id { get; set; }
    public int SortOrder { get; set; }

    // Navigation property for translations
    public required ICollection<BoardMemberTranslationModel> Translations { get; set; }
}