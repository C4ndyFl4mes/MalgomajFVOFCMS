using System.ComponentModel.DataAnnotations;

namespace Server.API.Routes.ImageFile.PUT;

public record PutImageRequest
{
    [Required]
    public required Guid Id { get; set; }
    [Required]
    public required Dictionary<string, string> Translations { get; set; } // Key: Language code, Value: Text.
}

public record PutImageResponse
{
    public required string Message { get; set; }
}