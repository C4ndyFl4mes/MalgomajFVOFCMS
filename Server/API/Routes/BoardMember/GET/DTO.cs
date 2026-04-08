using FluentValidation;

namespace Server.API.Routes.BoardMember.GET;

public record GetBoardMemberRequest
{
    public required string LanguageCode { get; set; }
}

public record GetBoardMemberResponse
{
    public required IEnumerable<BoardMemberDTO> BoardMembers { get; set; }
}

public record BoardMemberDTO
{
    public Guid Id { get; set; }
    public int SortOrder { get; set; }
    public required string Text { get; set; } // Already localized text for the requested language.
}

public sealed class GetBoardMemberRequestValidator : AbstractValidator<GetBoardMemberRequest>
{
    public GetBoardMemberRequestValidator()
    {
        RuleFor(x => x.LanguageCode)
            .NotEmpty().WithMessage("Språkkod är nödvändigt.")
            .Length(2).WithMessage("Språkkod måste vara två tecken långt.");
    }
}