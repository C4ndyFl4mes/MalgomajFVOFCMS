using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.ExternalMedia.DELETE;

public class DeleteExternalMediaEndpoint(AppDbContext ctx) : Endpoint<DeleteExternalMediaRequest, DeleteExternalMediaResponse>
{
    public override void Configure()
    {
        Delete("/api/externalmedia/{id}");
        Roles("Administrator", "Editor");
    }

    public override async Task<DeleteExternalMediaResponse> ExecuteAsync(DeleteExternalMediaRequest request, CancellationToken ct)
    {
        DeleteExternalMediaData data = new(ctx);

        await data.DeleteExternalMediaAsync(request.Id, ct);

        return DeleteExternalMediaMapper.MapToDeleteExternalMediaResponse(request.Id);
    }
}