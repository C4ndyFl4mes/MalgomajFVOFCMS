using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ImageFile.POST;

public class ImagePostEndpoint(AppDbContext ctx) : Endpoint<PostImageRequest, PostImageResponse>
{
    public override void Configure()
    {
        Post("api/image");
        Roles("Administrator", "Editor");
        AllowFileUploads();
        MaxRequestBodySize(10 * 1024 * 1024); // 10 MB
    }

    public override async Task<PostImageResponse> ExecuteAsync(PostImageRequest request, CancellationToken ct)
    {
        ImagePostData data = new(ctx, Resolve<IWebHostEnvironment>());

        ImageModel created = await data.PostImageAsync(request, ct);

        return ImagePostMapper.ToResponse(created);
    }
}