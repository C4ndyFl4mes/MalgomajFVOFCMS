using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.POST;

public class PostExternalMediaEndpoint(AppDbContext ctx) : Endpoint<PostExternalMediaRequest, PostExternalMediaResponse>
{
    public override void Configure()
    {
        Post("/api/externalmedia");
        Roles("Administrator", "Editor");
    }

    public override async Task<PostExternalMediaResponse> ExecuteAsync(PostExternalMediaRequest request, CancellationToken ct)
    {
        ExternalMediaModel media = PostExternalMediaMapper.MapToExternalMediaModel(request);
        
        PostExternalMediaData data = new(ctx);

        ExternalMediaModel createdMedia = await data.PostExternalMediaAsync(media, ct);

        return PostExternalMediaMapper.MapToPostExternalMediaResponse(createdMedia);
    }
}