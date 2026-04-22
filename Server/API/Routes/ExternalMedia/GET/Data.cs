using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.GET;

public class GetExternalMediaData(AppDbContext ctx)
{
    public async Task<ExternalMediaModel> GetExternalMediaModelAsync(Guid id, string language, CancellationToken ct)
    {
        ExternalMediaModel media = await ctx.ExternalMedia
            .Include(em => em.Translations)
            .FirstOrDefaultAsync(em => em.Id == id && em.Translations.Any(t => t.LanguageCode == language), ct) ??
                throw new KeyNotFoundException($"Ingen extern media hittades med ID '{id}' för språkkod '{language}'.");
        return media;
    }
}