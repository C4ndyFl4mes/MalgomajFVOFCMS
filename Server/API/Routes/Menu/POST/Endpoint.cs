using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.Menu.POST;

public class PostMenuEndpoint(AppDbContext ctx) : Endpoint<PostMenuRequest, PostMenuResponse>
{
    public override void Configure()
    {
        Post("api/menu/save");
        AllowAnonymous();
    }

    public override async Task<PostMenuResponse> ExecuteAsync(PostMenuRequest request, CancellationToken ct)
    {
        PostMenuData data = new(ctx);

        await data.PostMenuAsync(request, ct);

        return new PostMenuResponse
        {
            Message = "Lyckades spara huvudmenyn."
        };
    }
}