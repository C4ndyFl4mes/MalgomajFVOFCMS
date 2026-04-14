namespace Server.API.Models;

public sealed class SlideModel
{
    public Guid Id { get; set; }
    public required int SortOrder { get; set; }

    // A slide must always be an image.
    public required ImageModel Image { get; set; } = null!;
}