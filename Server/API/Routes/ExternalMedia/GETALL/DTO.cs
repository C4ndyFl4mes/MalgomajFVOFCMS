using FastEndpoints;
using FluentValidation;

namespace Server.API.Routes.ExternalMedia.GETALL;

public record GetAllExternalMediaResponse
{
    public required IEnumerable<GetExternalMediaResponse> ExternalMedia { get; set; }
}

public record GetExternalMediaResponse
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public required string Type { get; set; }
    public required string Text { get; set; } // Already localized text.
}


public record GetAllExternalMediaRequest
{
    public required string Language { get; set; }
}

public class GetAllExternalMediaRequestValidator : Validator<GetAllExternalMediaRequest>
{
    public GetAllExternalMediaRequestValidator()
    {
        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Språkkod är nödvändig.")
            .Length(2).WithMessage("Språkkoden måste vara exakt 2 tecken lång.");
    }
}