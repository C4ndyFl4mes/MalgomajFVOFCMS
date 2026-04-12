using FastEndpoints;
using FluentValidation;
using Server.API.Enums;

namespace Server.API.Routes.ExternalMedia.PUT;

public record PutExternalMediaRequest
{
    public required Guid Id { get; set; }
    public required string Url { get; set; }
    public required string Type { get; set; }
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text
}

public record PutExternalMediaResponse
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public required string Type { get; set; }
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text
}

public class PutExternalMediaRequestValidator : Validator<PutExternalMediaRequest>
{
    public PutExternalMediaRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id är nödvändigt.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("URL är nödvändigt.")
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("URL måste vara giltig.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Typ är nödvändigt.")
            .Must(type => Enum.TryParse<MediaType>(type, true, out _)).WithMessage("Typ måste vara en giltig MediaType.");

        RuleFor(x => x.Translations)
            .NotEmpty().WithMessage("Minst en översättning är nödvändig.")
            .Must(translations => translations.Keys.All(k => k.Length == 2)).WithMessage("Alla språkkoder måste vara två tecken långa.");
    }
}