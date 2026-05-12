using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.Menu.POST;

public class PostMenuEndpoint(AppDbContext ctx) : Endpoint<PostMenuRequest, PostMenuResponse>
{
    public override void Configure()
    {
        Post("api/menu/save");
        Roles("Administrator", "Editor");
    }

    public override async Task<PostMenuResponse> ExecuteAsync(PostMenuRequest request, CancellationToken ct)
    {
        PostMenuData data = new(ctx);

        return await data.PostMenuAsync(request, ct);
    }
}