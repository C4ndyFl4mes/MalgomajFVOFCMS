using FastEndpoints;
using FluentValidation;
using Server.API.Enums;

namespace Server.API.Routes.Page.POST;

public record PostPageRequest
{
    public Guid? Id { get; set; } // Optional for creating new pages, required for updating existing ones.
    public required string Type { get; set; }
    public bool IsPublished { get; set; } = false; 
    public DateTime SavedAt { get; set; } // Keeps track of when the page was last saved, it will be overriden when save succeeds.
    public required Dictionary<string, PostTranslationContentPageDTO> Translations { get; set; } // Key: LanguageCode, Value: Translation content.
}

// Provides default values since autosaving can give partial data. The Model will still be strict and require all fields.
public record PostTranslationContentPageDTO
{
    public string Title { get; set; } = "Namnlös sida"; 
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = "namnlos-sida"; 
    public string MetaDescription { get; set; } = string.Empty;
    public string MetaKeywords { get; set; } = string.Empty;
}

// Since the database just needs to mirror the editor, we don't need to send back complex data.
public record PostPageResponse
{
    public required string Message { get; set; }
    public required bool IsPublished { get; set; }
    public required DateTime SavedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } // When the page is saved after being published.
}

public class PostPageRequestValidator : Validator<PostPageRequest>
{
    public PostPageRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Sidtyp är nödvändig.")
            .Must(type => Enum.TryParse<PageType>(type, true, out _))
            .WithMessage("Ogiltig sidtyp. Tillåtna värden är: Home, Content, Contact.");
        
        RuleFor(x => x.Translations.Keys)
            .NotEmpty().WithMessage("Minst en översättning krävs.")
            .Must(keys => keys.All(lang => lang.Length == 2))
            .WithMessage("Alla språkkoder måste vara exakt 2 tecken långa.");

        RuleForEach(x => x.Translations.Values)
            .SetValidator(new PostTranslationContentPageDTOValidator());
    }
}

public class PostTranslationContentPageDTOValidator : Validator<PostTranslationContentPageDTO>
{
    public PostTranslationContentPageDTOValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(150).WithMessage("Titeln får inte vara längre än 150 tecken.");
        
        RuleFor(x => x.Slug)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug måste vara i formatet 'exempel-slug' och får endast innehålla små bokstäver, siffror och bindestreck.");
        
        RuleFor(x => x.MetaDescription)
            .MaximumLength(300).WithMessage("Meta-beskrivningen får inte vara längre än 300 tecken.");
        
        RuleFor(x => x.MetaKeywords)
            .MaximumLength(300).WithMessage("Meta-nyckelorden får inte vara längre än 300 tecken.");
    }
}