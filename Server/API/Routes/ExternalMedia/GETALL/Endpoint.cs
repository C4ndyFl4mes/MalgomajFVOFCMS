using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.GETALL;

public sealed class GetAllExternalMediaEndpoint(AppDbContext ctx) : Endpoint<GetAllExternalMediaRequest, GetAllExternalMediaResponse>
{
    public override void Configure()
    {
        Get("/api/externalmedia");
        AllowAnonymous();
    }

    public override async Task<GetAllExternalMediaResponse> ExecuteAsync(GetAllExternalMediaRequest request, CancellationToken ct)
    {
        GetAllExternalMediaData data = new GetAllExternalMediaData(ctx);

        IEnumerable<ExternalMediaModel> media = await data.GetAllExternalMediaAsync(request.Language, ct);

        return GetAllExternalMediaMapper.MapToGetAllExternalMediaResponse(media, request.Language);
    }
}