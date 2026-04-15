using FastEndpoints;
using FluentValidation;

namespace Server.API.Routes.Page.GET.Editor;


public record GetPageEditorRequest
{
    public required Guid PageId { get; set; }
}

public record GetPageEditorResponse
{
    public required Guid PageId { get; set; }
    public required string Type { get; set; }
    public required bool IsPublished { get; set; }
    public required DateTime SavedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public required Dictionary<string, GetTranslationContentPageDTO> Translations { get; set; } // Key: LanguageCode, Value: Translation content.
}

public record GetTranslationContentPageDTO
{
    public required string Title { get; set; }
    public string? Content { get; set; } = string.Empty;
    public required string Slug { get; set; }
    public string? MetaDescription { get; set; } = string.Empty;
    public string? MetaKeywords { get; set; } = string.Empty;
}

public class GetPageEditorRequestValidator : Validator<GetPageEditorRequest>
{
    public GetPageEditorRequestValidator()
    {
        RuleFor(x => x.PageId)
            .NotEmpty().WithMessage("SidId är nödvändig.");
    }
}