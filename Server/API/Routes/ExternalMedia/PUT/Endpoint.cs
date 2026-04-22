using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.PUT;

public class PutExternalMediaEndpoint(AppDbContext ctx) : Endpoint<PutExternalMediaRequest, PutExternalMediaResponse>
{
    public override void Configure()
    {
        Put("/api/externalmedia");
        Roles("Administrator", "Editor");
    }

    public override async Task<PutExternalMediaResponse> ExecuteAsync(PutExternalMediaRequest request, CancellationToken ct)
    {
        ExternalMediaModel media = PutExternalMediaMapper.MapToExternalMediaModel(request);
        
        PutExternalMediaData data = new(ctx);

        ExternalMediaModel updatedMedia = await data.PutExternalMediaAsync(media, ct);

        return PutExternalMediaMapper.MapToPutExternalMediaResponse(updatedMedia);
    }
}