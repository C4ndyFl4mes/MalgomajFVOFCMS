using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.Menu.GET.State;

public class GetMenuStateEndpoint(AppDbContext ctx) : EndpointWithoutRequest<GetMenuStateResponse>
{
    public override void Configure()
    {
        Get("api/menu/state");
        AllowAnonymous();
    }

    public override async Task<GetMenuStateResponse> ExecuteAsync(CancellationToken ct)
    {
        GetMenuStateData data = new(ctx);

        return await data.GetMenuStateAsync(ct);
    }
}