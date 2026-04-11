using Server.API.Models;

namespace Server.API.Routes.ImageFile.GET;

public static class GetImagesMapper
{
    public static GetImagesResponse ToResponse(List<ImageModel> images)
    {
        return new GetImagesResponse
        {
            Images = images.Select(image => new ImageDTO
            {
                Id = image.Id,
                Type = image.Type.ToString(),
                Translations = image.Translations.Select(t => new 
                {
                    t.LanguageCode,
                    t.Alt
                }).ToDictionary(t => t.LanguageCode, t => t.Alt)
            }).ToList()
        };
    }
}