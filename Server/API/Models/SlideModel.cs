namespace Server.API.Models;

public sealed class SlideModel
{
    public Guid Id { get; set; }
    public int SortOrder { get; set; }


    // Navigation property for the associated ImageModel (one-to-one relationship)
    public Guid ImageId { get; set; }
    public required ImageModel Image { get; set; }
}