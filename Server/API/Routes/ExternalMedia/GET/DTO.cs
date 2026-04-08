using FluentValidation;

namespace Server.API.Routes.ExternalMedia.GET;

public record GetExternalMediaResponse
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public required string Type { get; set; }
    public required string Text { get; set; } // Already localized text.
}

public record GetExternalMediaRequest
{
    public Guid Id { get; set; }
    public required string Language { get; set; }
}

public class GetExternalMediaRequestValidator : AbstractValidator<GetExternalMediaRequest>
{
    public GetExternalMediaRequestValidator()
    {
        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Språkkod är nödvändig.")
            .Length(2).WithMessage("Språkkoden måste vara exakt 2 tecken lång.");
    }
}