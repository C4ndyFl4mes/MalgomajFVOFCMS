using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Page.POST;

public class PostPageEndpoint(AppDbContext ctx) : Endpoint<PostPageRequest, PostPageResponse>
{
    public override void Configure()
    {
        Post("api/page");
        AllowAnonymous();
    }

    public override async Task<PostPageResponse> ExecuteAsync(PostPageRequest request, CancellationToken ct)
    {
        PostPageData data = new(ctx);

        PageModel page = await data.SavePageAsync(request, ct);

        return PostPageMapper.ToResponse(page);
    }
}