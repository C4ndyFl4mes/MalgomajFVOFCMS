using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ExternalMedia.PUT;

public sealed class PutExternalMediaData(AppDbContext ctx)
{
    public async Task<ExternalMediaModel> PutExternalMediaAsync(ExternalMediaModel media, CancellationToken ct)
    {
        ExternalMediaModel existingMedia = await ctx.ExternalMedia
            .Include(m => m.Translations)
            .FirstOrDefaultAsync(m => m.Id == media.Id, ct) ??
                throw new KeyNotFoundException($"Extern media med ID {media.Id} kunde inte hittas.");

        existingMedia.Url = media.Url;
        existingMedia.Type = media.Type;

        // Update translations
        existingMedia.Translations.Clear();
        foreach (ExternalMediaTranslationModel translation in media.Translations)
        {
            existingMedia.Translations.Add(new ExternalMediaTranslationModel
            {
                LanguageCode = translation.LanguageCode,
                Text = translation.Text,
                ExternalMedia = existingMedia
            });
        }

        await ctx.SaveChangesAsync(ct);
        return existingMedia;
    }
}