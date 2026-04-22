using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Page.GET.Editor;

public class GetPageEditorEndpoint(AppDbContext ctx) : Endpoint<GetPageEditorRequest, GetPageEditorResponse>
{
    public override void Configure()
    {
        Get("api/page/{pageId}");
        Roles("Administrator", "Editor");
    }

    public override async Task<GetPageEditorResponse> ExecuteAsync(GetPageEditorRequest request, CancellationToken ct)
    {
        GetPageEditorData data = new(ctx);

        PageModel model = await data.GetPageEditorAsync(request, ct);

        return GetPageEditorMapper.ToResponse(model);
    }
}