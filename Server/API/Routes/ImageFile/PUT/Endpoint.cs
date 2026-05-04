using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.ImageFile.PUT;

public class ImagePutEndpoint(AppDbContext ctx) : Endpoint<PutImageRequest, PutImageResponse>
{
    public override void Configure()
    {
        Put("/api/images/");
        AllowAnonymous();
    }

    public override async Task<PutImageResponse> ExecuteAsync(PutImageRequest request, CancellationToken ct)
    {
        ImagePutData data = new(ctx);
        
        return await data.UpdateImageAsync(request, ct);
    }
}