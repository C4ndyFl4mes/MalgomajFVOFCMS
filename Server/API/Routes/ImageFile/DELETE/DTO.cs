namespace Server.API.Routes.ImageFile.DELETE;

public record DeleteImageRequest
{
    public required Guid Id { get; set; }
}

public record DeleteImageResponse
{
    public required Guid Id { get; set; }
    public required string Message { get; set; }
}