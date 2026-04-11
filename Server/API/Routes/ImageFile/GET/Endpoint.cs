using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ImageFile.GET;

public class GetImagesEndpoint(AppDbContext ctx) : EndpointWithoutRequest<GetImagesResponse>
{
    public override void Configure()
    {
        Get("/api/images");
        AllowAnonymous();
    }

    public override async Task<GetImagesResponse> ExecuteAsync(CancellationToken ct)
    {
        GetImagesData data = new(ctx, Resolve<IWebHostEnvironment>());
        List<ImageModel> images = await data.GetAllImagesAsync(ct);
        return GetImagesMapper.ToResponse(images);
    }
}