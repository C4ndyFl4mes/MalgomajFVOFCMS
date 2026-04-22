using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.GET;

public class GetExternalMediaEndpoint(AppDbContext ctx) : Endpoint<GetExternalMediaRequest, GetExternalMediaResponse>
{
    public override void Configure()
    {
        Get("/api/externalmedia/{id}");
        AllowAnonymous();
    }

    public override async Task<GetExternalMediaResponse> ExecuteAsync(GetExternalMediaRequest request, CancellationToken ct)
    {
        GetExternalMediaData data = new GetExternalMediaData(ctx);

        ExternalMediaModel media = await data.GetExternalMediaModelAsync(request.Id, request.Language, ct);

        return GetExternalMediaMapper.MapToGetExternalMediaResponse(media, request.Language);
    }
}