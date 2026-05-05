namespace Server.UI.Components;

public record TranslationFieldModel
{
    public required string LanguageCode { get; set; }
    public required string Text { get; set; }
    public required string OldLanguageCode { get; set; }
}