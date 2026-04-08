using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.DELETE;

public sealed class DeleteExternalMediaData(AppDbContext ctx)
{
    public async Task DeleteExternalMediaAsync(Guid id, CancellationToken ct)
    {
        ExternalMediaModel media = await ctx.ExternalMedia.FindAsync(id, ct) ??
            throw new KeyNotFoundException($"Extern media finns inte. ID: {id}");

        ctx.ExternalMedia.Remove(media);
        await ctx.SaveChangesAsync(ct);
    }
}