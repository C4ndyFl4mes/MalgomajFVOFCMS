using Microsoft.AspNetCore.Components.Forms;
using Server.API.Data;
using Server.API.Enums;
using Server.API.Exceptions;
using Server.API.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Server.API.Routes.ImageFile.POST;

public class ImagePostData(AppDbContext ctx, IWebHostEnvironment env)
{
    private readonly ImageConfig _config = new();

    public async Task<ImageModel> PostImageAsync(PostImageRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse(request.Type, true, out ImageType imageType))
            throw new ArgumentOutOfRangeException(nameof(request.Type), $"Ogiltig bildtyp: {request.Type}");
        
        Guid imageId = Guid.NewGuid();
        string imageRoot = Path.Combine(env.WebRootPath, "images", imageId.ToString());

        bool isSvg = IsSvg(request.ImageFile);

        if (imageType == ImageType.Icon)
        {
            if (!isSvg)
                throw new BadRequestException("Ikoner måste laddas upp som SVG.");

            string svgDirectory = Path.Combine(imageRoot, "svg");
            Directory.CreateDirectory(svgDirectory);

            string svgPath = Path.Combine(svgDirectory, "icon.svg");

            await using Stream target = File.Create(svgPath);
            await request.ImageFile.OpenReadStream(10_000_000).CopyToAsync(target, ct);
        }
        else
        {
            if (isSvg)
                throw new BadRequestException("Endast ikoner kan laddas upp som SVG.");
            
            Directory.CreateDirectory(Path.Combine(imageRoot, "jpg"));
            Directory.CreateDirectory(Path.Combine(imageRoot, "webp"));
            
            using Image source = await Image.LoadAsync(request.ImageFile.OpenReadStream(10_000_000), ct);

            await SaveVariantAsync(source, imageRoot, imageType, "desktop", false, ct);
            await SaveVariantAsync(source, imageRoot, imageType, "mobile", true, ct);
        }

        ImageModel image = new()
        {
            Id = imageId,
            Type = imageType,
            Slide = null!, // This will be set if the image is associated with a slide later.
            Translations = request.Translations.Select(t => new ImageAltTranslationModel
            {
                ImageId = imageId,
                LanguageCode = t.Key,
                Alt = t.Value,
                Image = null! // This will be set by EF Core when the ImageModel is saved.
            }).ToList()
        };
        
        await ctx.Images.AddAsync(image, ct);
        await ctx.SaveChangesAsync(ct);

        return image;
    }

    // Check if the uploaded file is an SVG based on its extension and content type.
    private static bool IsSvg(IBrowserFile file)
    {
        string extension = Path.GetExtension(file.Name).ToLowerInvariant();
        string contentType = file.ContentType.ToLowerInvariant();

        return extension == ".svg" || contentType == "image/svg+xml";
    }

    // Save a resized variant of the image for desktop or mobile.
    private async Task SaveVariantAsync(Image source, string imageRoot, ImageType imageType, string sizeName, bool isMobile, CancellationToken ct)
    {
        (int width, int height) = _config.GetDimensions(imageType, isMobile);

        using Image resized = source.Clone(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = imageType == ImageType.Banner ? ResizeMode.Crop : ResizeMode.Max,
            Position = AnchorPositionMode.Center
        }));

        string jpegPath = Path.Combine(imageRoot, "jpg", $"{sizeName}.jpg");
        string webpPath = Path.Combine(imageRoot, "webp", $"{sizeName}.webp");

        await resized.SaveAsJpegAsync(jpegPath, new JpegEncoder { Quality = 85, SkipMetadata = true }, ct);
        await resized.SaveAsWebpAsync(webpPath, new WebpEncoder { Quality = 75, SkipMetadata = true }, ct);
    }
}

// Configuration class to define dimensions for different image types and handle scaling for mobile variants. Doesn't need Icon type since icons are always SVG and not resized.
public class ImageConfig {
    private const double SmallScale = 0.5;

    private (int width, int height) Banner = (1200, 400); // 3:1
    private (int width, int height) Normal = (800, 450); // 16:9
    private (int width, int height) Square = (300, 300); // 1:1

    public (int width, int height) GetDimensions(ImageType type, bool isMobile = false)
    {
        (int width, int height) dimensions = type switch
        {
            ImageType.Banner => Banner,
            ImageType.Normal => Normal,
            ImageType.Square => Square,
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Ogiltig bildtyp: {type}")
        };

        return isMobile ? ScaleDimensions(dimensions) : dimensions;
    }

    private (int width, int height) ScaleDimensions((int width, int height) dimensions)
    {
        return ((int)(dimensions.width * SmallScale), (int)(dimensions.height * SmallScale));
    }
}