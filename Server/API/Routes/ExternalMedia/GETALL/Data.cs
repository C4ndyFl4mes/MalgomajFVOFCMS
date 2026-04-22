using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Exceptions;
using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.GETALL;

public class GetAllExternalMediaData(AppDbContext ctx)
{
    public async Task<IEnumerable<ExternalMediaModel>> GetAllExternalMediaAsync(string language, CancellationToken ct)
    {
        IEnumerable<ExternalMediaModel> media = await ctx.ExternalMedia
            .Include(em => em.Translations)
            .Where(em => em.Translations.Any(t => t.LanguageCode == language))
            .ToListAsync(ct);
        
        if (!media.Any())
        {
            throw new NotFoundException($"Ingen extern media hittades för språkkod '{language}'.");
        }
        return media;
    }
}