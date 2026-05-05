using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.ImageFile.GET;

public class GetImagesData(AppDbContext ctx, IWebHostEnvironment env)
{
    public async Task<GetImagesResponse> GetAllImagesAsync(string[] imageTypes, CancellationToken ct)
    {
        string imagesRoot = Path.Combine(env.WebRootPath, "images");
        if (!Directory.Exists(imagesRoot))
        {
            throw new DirectoryNotFoundException("Mappen för bilder hittades inte på servern.");
        }

        if (Directory.GetDirectories(imagesRoot).Length == 0)
        {
            throw new KeyNotFoundException("Inga bilder hittades på servern.");
        }

        if (!ctx.Images.Any())
        {
            throw new KeyNotFoundException("Inga bilder hittades i databasen.");
        }

        List<ImageModel> images = await ctx.Images.Include(i => i.Translations).Where(i => imageTypes.Contains(i.Type.ToString())).ToListAsync(ct);
        
        return GetImagesMapper.ToResponse(images);
    }
}