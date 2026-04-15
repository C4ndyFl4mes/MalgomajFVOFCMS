using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Page.GET.List;

public class GetPageListEndpoint(AppDbContext ctx) : EndpointWithoutRequest<GetPageListResponse>
{
    public override void Configure()
    {
        Get("api/pages");
        AllowAnonymous();
    }

    public override async Task<GetPageListResponse> ExecuteAsync(CancellationToken ct)
    {
        GetPageListData data = new(ctx);

        List<PageModel> models = await data.GetPageListAsync(ct);

        return GetPageListMapper.ToResponse(models);
    }
}