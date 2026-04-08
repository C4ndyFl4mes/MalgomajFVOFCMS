using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.GETALL;

public static class GetAllExternalMediaMapper
{
    public static GetAllExternalMediaResponse MapToGetAllExternalMediaResponse(IEnumerable<ExternalMediaModel> media, string language)
    {
        return new GetAllExternalMediaResponse
        {
            ExternalMedia = media.Select(em => new GetExternalMediaResponse
            {
                Id = em.Id,
                Url = em.Url,
                Type = em.Type.ToString(),
                Text = em.Translations.FirstOrDefault(t => t.LanguageCode == language)?.Text ?? string.Empty 
            }).ToList()
        };
    }
}