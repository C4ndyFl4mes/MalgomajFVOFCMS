using FastEndpoints;
using FluentValidation;
using Server.API.Enums;

namespace Server.API.Routes.ExternalMedia.POST;

public record PostExternalMediaRequest
{
    public required string Url { get; set; }
    public required string Type { get; set; }
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text
}

public record PostExternalMediaResponse
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public required string Type { get; set; }
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text
}

public class PostExternalMediaRequestValidator : Validator<PostExternalMediaRequest>
{
    public PostExternalMediaRequestValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("URL är nödvändigt.")
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("URL måste vara giltig.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Typ är nödvändigt.")
            .Must(type => Enum.TryParse<MediaType>(type, true, out _)).WithMessage("Typ måste vara en giltig MediaType.");

        RuleFor(x => x.Translations)
            .NotEmpty().WithMessage("Åtminstone en språkversion ska finnas.")
            .Must(translations => translations.Keys.All(k => k.Length == 2)).WithMessage("Alla språkkoder måste vara två tecken långa.")
            .Must(translations => translations.Values.All(v => !string.IsNullOrWhiteSpace(v))).WithMessage("Alla översättningar måste ha text.");
    }
}