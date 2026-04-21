namespace Server.API.Routes.User.Refresh;

public record RefreshRequest
{
    public required Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}

public record Token
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}

public record RefreshResponse
{
    public required string Message { get; set; }
}