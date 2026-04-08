using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.POST;

public sealed class PostExternalMediaData(AppDbContext ctx)
{
    public async Task<ExternalMediaModel> PostExternalMediaAsync(ExternalMediaModel media, CancellationToken ct)
    {
        ctx.ExternalMedia.Add(media);
        await ctx.SaveChangesAsync(ct);
        return media;
    }
}