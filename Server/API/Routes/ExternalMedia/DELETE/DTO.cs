using FastEndpoints;
using FluentValidation;

namespace Server.API.Routes.ExternalMedia.DELETE;

public record DeleteExternalMediaRequest
{
    public required Guid Id { get; set; }
}

public record DeleteExternalMediaResponse
{
    public required Guid Id { get; set; }
    public required string Message { get; set; }
}

public class DeleteExternalMediaRequestValidator : Validator<DeleteExternalMediaRequest>
{
    public DeleteExternalMediaRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id är nödvändigt.");
    }
}