namespace Server.API.Routes.User.DELETE;

public record DeleteUserRequest
{
    public required Guid UserId { get; set; }
}

public record DeleteUserResponse
{
    public required Guid UserId { get; set; }
    public required string Message { get; set; }
}