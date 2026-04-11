using Server.API.Models;

namespace Server.API.Routes.ImageFile.POST;

public static class ImagePostMapper
{
    public static PostImageResponse ToResponse(ImageModel model)
    {
        return new PostImageResponse
        {
            Id = model.Id,
            Type = model.Type.ToString()
        };
    }
}