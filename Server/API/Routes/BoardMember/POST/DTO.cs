using FastEndpoints;
using FluentValidation;

namespace Server.API.Routes.BoardMember.POST;

public record PostBoardMemberRequest
{
    public required int SortOrder { get; set; }
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text.
}

public record PostBoardMemberResponse
{
    public required Guid Id { get; set; }
    public required int SortOrder { get; set; }
    public required Dictionary<string, string> Translations {get; set; } // Key: Language code, Value: Text.
}

public class PostBoardMemberRequestValidator : Validator<PostBoardMemberRequest>
{
    public PostBoardMemberRequestValidator()
    {
        RuleFor(x => x.Translations)
            .NotEmpty().WithMessage("Åtminstone en språkversion ska finnas.")
            .Must(translations => translations.Keys.All(k => k.Length == 2)).WithMessage("Alla språkkoder måste vara två tecken långa.")
            .Must(translations => translations.Values.All(v => !string.IsNullOrWhiteSpace(v))).WithMessage("Alla översättningar måste ha text.");
    }
}