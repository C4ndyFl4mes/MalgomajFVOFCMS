using Server.API.Enums;
using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.PUT;

public static class PutExternalMediaMapper
{
    public static ExternalMediaModel MapToExternalMediaModel(PutExternalMediaRequest request)
    {
        List<ExternalMediaTranslationModel> translations = [];
        foreach (string language in request.Translations.Keys)
        {
            translations.Add(new ExternalMediaTranslationModel
            {
                LanguageCode = language,
                Text = request.Translations[language],
                ExternalMedia = null! // This will be set by EF Core when we add the ExternalMediaModel to the context.
            });
        }

        return new ExternalMediaModel
        {
            Id = request.Id,
            Url = request.Url,
            Type = Enum.Parse<MediaType>(request.Type, true),
            Translations = translations
        };
    }

    public static PutExternalMediaResponse MapToPutExternalMediaResponse(ExternalMediaModel model)
    {
        Dictionary<string, string> translations = [];
        foreach (ExternalMediaTranslationModel translation in model.Translations)
        {
            translations[translation.LanguageCode] = translation.Text;
        }

        return new PutExternalMediaResponse
        {
            Id = model.Id,
            Url = model.Url,
            Type = model.Type.ToString(),
            Translations = translations
        };
    }
}