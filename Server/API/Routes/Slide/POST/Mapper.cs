using Server.API.Models;

namespace Server.API.Routes.Slide.POST;

public static class PostSlideMapper
{
    public static List<SlideModel> MapToSlideModelList(PostSlideRequest request)
    {
        List<SlideModel> slideModels = request.Slides.Select(slide => new SlideModel
        {
            Id = slide.ImageID,
            SortOrder = slide.SortOrder,
            Image = null! // This will be set by EF Core when we add 
        }).ToList();

        return slideModels;
    }
}