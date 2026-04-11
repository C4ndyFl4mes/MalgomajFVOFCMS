using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ImageFile.DELETE;

public class DeleteImageData(AppDbContext ctx, IWebHostEnvironment env)
{
    public async Task DeleteImageAsync(Guid id, CancellationToken ct)
    {
        // Deletes the image record from the database.
        ImageModel image = await ctx.Images.FindAsync(id, ct) ??
            throw new KeyNotFoundException($"Bild med ID {id} hittades inte i databasen.");
        
        ctx.Images.Remove(image);
        await ctx.SaveChangesAsync(ct);
        
        // Deletes the image files from the server.
        string imageRoot = Path.Combine(env.WebRootPath, "images", id.ToString());
        if (Directory.Exists(imageRoot))
        {
            Directory.Delete(imageRoot, true);
        }
    }
}