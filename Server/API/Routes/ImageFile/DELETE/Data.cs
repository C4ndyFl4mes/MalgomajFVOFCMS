using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ImageFile.DELETE;

public class DeleteImageData(AppDbContext ctx, IWebHostEnvironment env)
{
    public async Task DeleteImageAsync(Guid id, CancellationToken ct)
    {
        // Raderar bildinformationen från databasen.
        ImageModel image = await ctx.Images.FindAsync([id, ct], cancellationToken: ct) ??
            throw new KeyNotFoundException($"Bild med ID {id} hittades inte i databasen.");
        
        ctx.Images.Remove(image);
        await ctx.SaveChangesAsync(ct);
        
        // Raderar bildfilen från filsystemet.
        string imageRoot = Path.Combine(env.WebRootPath, "images", id.ToString());
        if (Directory.Exists(imageRoot))
        {
            Directory.Delete(imageRoot, true);
        }
    }
}