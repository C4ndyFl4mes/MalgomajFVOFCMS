using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.GET;

public static class GetExternalMediaMapper
{
    public static GetExternalMediaResponse MapToGetExternalMediaResponse(ExternalMediaModel media, string language)
    {
        return new GetExternalMediaResponse
        {
            Id = media.Id,
            Url = media.Url,
            Type = media.Type.ToString(),
            Text = media.Translations.FirstOrDefault(t => t.LanguageCode == language)?.Text ?? string.Empty 
        };
    }
}