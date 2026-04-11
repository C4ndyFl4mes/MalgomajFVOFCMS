using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.ImageFile.DELETE;

public class DeleteImageEndpoint(AppDbContext ctx) : Endpoint<DeleteImageRequest, DeleteImageResponse>
{
    public override void Configure()
    {
        Delete("/api/image/{id}");
        AllowAnonymous();
    }

    public override async Task<DeleteImageResponse> ExecuteAsync(DeleteImageRequest request, CancellationToken ct)
    {
        DeleteImageData data = new(ctx, Resolve<IWebHostEnvironment>());
        await data.DeleteImageAsync(request.Id, ct);
        return DeleteImageMapper.ToResponse(request.Id);
    }
}