namespace Server.API.Routes.User.Refresh;

public record RefreshRequest
{
    public required Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}

public record Token
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public bool IsEmpty => string.IsNullOrEmpty(AccessToken) || string.IsNullOrEmpty(RefreshToken);
}

public record RefreshResponse
{
    public required string Message { get; set; }
}