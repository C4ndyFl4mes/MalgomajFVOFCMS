namespace Server.API.Routes.Slide.GET;

public record GetSlideshowRequest
{
    public required string LanguageCode { get; set; }
}

public record GetSlideshowResponse
{
    public required List<GetSlideDTO> Slides { get; set; }
}

public record GetSlideDTO
{
    public required Guid ImageID { get; set; }
    public required int SortOrder { get; set; }
    public required string Alt { get; set; } // Already localized.
}