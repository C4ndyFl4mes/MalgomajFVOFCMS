using FluentValidation;

namespace Server.API.Routes.BoardMember.DELETE;

public record DeleteBoardMemberRequest
{
    public required Guid Id { get; set; }
}

public record DeleteBoardMemberResponse
{
    public required Guid Id { get; set; }
    public required string Message { get; set; }
}

public class DeleteBoardMemberRequestValidator : AbstractValidator<DeleteBoardMemberRequest>
{
    public DeleteBoardMemberRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id är nödvändigt.");
    }
}