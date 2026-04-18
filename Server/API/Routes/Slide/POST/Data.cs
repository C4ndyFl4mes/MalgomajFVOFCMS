using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Slide.POST;

public class PostSlideData(AppDbContext ctx)
{
    public async Task PostSlideAsync(List<SlideModel> slides, CancellationToken ct)
    {
        await using var transaction = await ctx.Database.BeginTransactionAsync(ct);

        List<ImageModel> banners = await ctx.Images.Where(image => image.Type == Enums.ImageType.Banner).ToListAsync(ct);

        if (!banners.Exists(banner => slides.Any(slide => banner.Id == slide.Id)))
            throw new KeyNotFoundException("Hittade inte en motsvarande banderollbild för en eller flera bilder. Endast banderollbilder kan användas i bildspelet.");

        List<SlideModel> slideModels = await ClearAsync(ct);

        foreach (SlideModel slide in slides)
        {
            foreach (ImageModel banner in banners)
            {
                if (slide.Id == banner.Id)
                {
                    slide.Image = banner;
                }
            }
        }

        slideModels.AddRange(slides);

        await ctx.Slides.AddRangeAsync(slideModels, ct);

        await ctx.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    private async Task<List<SlideModel>> ClearAsync(CancellationToken ct)
    {
        List<SlideModel> existingSlides = await ctx.Slides.ToListAsync(ct);
        ctx.Slides.RemoveRange(existingSlides);
        existingSlides.Clear();

        return existingSlides;
    }
}