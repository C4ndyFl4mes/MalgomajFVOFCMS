using FluentValidation;

namespace Server.API.Routes.BoardMember.PUT;

public record PutBoardMemberRequest
{
    public required Guid Id { get; set; }
    public required int SortOrder { get; set; }
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text.
}

public record PutBoardMemberResponse
{
    public Guid Id { get; set; }
    public required int SortOrder { get; set; }
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text.
}

public class PutBoardMemberRequestValidator : AbstractValidator<PutBoardMemberRequest>
{
    public PutBoardMemberRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id är nödvändigt.");
    
        RuleFor(x => x.Translations)
            .NotEmpty().WithMessage("Åtminstone en språkversion ska finnas.")
            .Must(translations => translations.Keys.All(k => k.Length == 2)).WithMessage("Alla språkkoder måste vara två tecken långa.")
            .Must(translations => translations.Values.All(v => !string.IsNullOrWhiteSpace(v))).WithMessage("Alla översättningar måste ha text.");
    }
}