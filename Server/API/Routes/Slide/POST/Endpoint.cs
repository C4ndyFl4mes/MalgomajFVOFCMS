using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.Slide.POST;

public class PostSlideEndpoint(AppDbContext ctx) : Endpoint<PostSlideRequest, PostSlideResponse>
{
    public override void Configure()
    {
        Post("/api/slide");
        AllowAnonymous();
    }

    public override async Task<PostSlideResponse> ExecuteAsync(PostSlideRequest request, CancellationToken ct)
    {
        PostSlideData data = new(ctx);

        await data.PostSlideAsync(PostSlideMapper.MapToSlideModelList(request), ct);

        return new PostSlideResponse
        {
            Message = "Bildspel sparades."
        };
    }
}