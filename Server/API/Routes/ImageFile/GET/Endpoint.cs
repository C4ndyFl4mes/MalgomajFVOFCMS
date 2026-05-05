using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ImageFile.GET;

public class GetImagesEndpoint(AppDbContext ctx) : Endpoint<GetImagesRequest, GetImagesResponse>
{
    public override void Configure()
    {
        Get("/api/images");
        Roles("Administrator", "Editor");
    }

    public override async Task<GetImagesResponse> ExecuteAsync(GetImagesRequest request, CancellationToken ct)
    {
        GetImagesData data = new(ctx, Resolve<IWebHostEnvironment>());

        return await data.GetAllImagesAsync(request.Types, ct);
    }
}