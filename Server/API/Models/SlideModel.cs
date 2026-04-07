namespace Server.API.Models;

public sealed class SlideModel
{
    public Guid ImageId { get; set; }
    public required ImageModel Image { get; set; }
    public int SortOrder { get; set; }
}