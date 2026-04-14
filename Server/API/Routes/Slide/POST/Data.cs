using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Slide.POST;

public class PostSlideData(AppDbContext ctx)
{
    public async Task PostSlideAsync(List<SlideModel> slides, CancellationToken ct)
    {
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

        foreach (SlideModel slide in slideModels)
        {
            Console.WriteLine(slide.Id);
        }

        await ctx.Slides.AddRangeAsync(slideModels, ct);

        int savedChanges = await ctx.SaveChangesAsync(ct);


        Console.WriteLine(savedChanges);






        // List<ImageModel> images = await ctx.Images.Where(i => request.Slides.Any(slide => slide.ImageID == i.Id)).ToListAsync(ct);
        // List<ImageModel> existingSlides = await ctx.Images.Include(image => image.Slide).Where(image => image.Slide != null && image.Type == Enums.ImageType.Banner).ToListAsync(ct);

        // List<ImageModel> images = await ctx.Images.Where(image => 

        // Move to get later.
        // if (images.All(image => request.Slides.Any(slide => slide.ImageID == image.Id)))
        //     throw new BadRequestException("En eller flera bild ID:er finns inte.");

        // if (images)

        // foreach (ImageModel image in images)
        // {
        //     Console.WriteLine(image.Id);
        // }
    }

    private async Task<List<SlideModel>> ClearAsync(CancellationToken ct)
    {
        List<SlideModel> existingSlides = await ctx.Slides.ToListAsync(ct);
        ctx.Slides.RemoveRange(existingSlides);
        existingSlides.Clear();

        await ctx.SaveChangesAsync(ct);

        return existingSlides;
    }
}