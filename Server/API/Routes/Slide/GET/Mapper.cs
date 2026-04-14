using Server.API.Models;

namespace Server.API.Routes.Slide.GET;

public static class GetSlideshowMapper
{
    public static GetSlideshowResponse ToResponse(List<SlideModel> slides, string language)
    {
        return new GetSlideshowResponse
        {
            Slides = slides.Select(slide => new GetSlideDTO
            {
                ImageID = slide.Id,
                SortOrder = slide.SortOrder,
                Alt = slide.Image.Translations.FirstOrDefault(t => t.LanguageCode == language)?.Alt ?? string.Empty
            }).ToList()
        };
    }
}