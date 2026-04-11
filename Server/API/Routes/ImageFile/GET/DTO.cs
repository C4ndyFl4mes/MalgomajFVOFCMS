using FluentValidation;

namespace Server.API.Routes.ImageFile.GET;

public record ImageDTO
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text.
}

public record GetImagesResponse
{
    public required IEnumerable<ImageDTO> Images { get; set; }
}