using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Slide.GET;

public class GetSlideshowData(AppDbContext ctx)
{
    public async Task<List<SlideModel>> GetSlideshowAsync(CancellationToken ct)
    {
        List<SlideModel> slides = await ctx.Slides.Include(slide => slide.Image.Translations).ToListAsync(ct);

        return slides;
    }
}