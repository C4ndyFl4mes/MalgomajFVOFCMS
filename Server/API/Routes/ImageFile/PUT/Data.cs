using Microsoft.EntityFrameworkCore.Storage;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ImageFile.PUT;

public class ImagePutData(AppDbContext ctx)
{
    public async Task<PutImageResponse> UpdateImageAsync(PutImageRequest request, CancellationToken ct)
    {
        IDbContextTransaction transaction = await ctx.Database.BeginTransactionAsync(ct);

        ImageModel image = await ctx.Images.FindAsync([request.Id], ct) ??
            throw new KeyNotFoundException($"Ingen bild hittades med ID: {request.Id}");
        
        ctx.ImageAltTranslations.RemoveRange(image.Translations);

        image.Translations = request.Translations.Select(t => new ImageAltTranslationModel
        {
            ImageId = image.Id,
            LanguageCode = t.Key,
            Alt = t.Value,
            Image = null! // Detta kommer att sättas av EF Core när ImageModel sparas.
        }).ToList();

        await ctx.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return new PutImageResponse
        {
            Message = "Bild uppdaterad."
        };
    }
}