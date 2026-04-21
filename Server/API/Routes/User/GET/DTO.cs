namespace Server.API.Routes.User.GET;

public record GetUserRequest
{
    public Guid? UserId { get; set; } // Optional: If provided, fetch specific user; otherwise, fetch all users.
}

public record UserDTO
{
    public required Guid UserId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string RoleName { get; set; }
}

public record GetUsersResponse
{
    public required List<UserDTO> Users { get; set; }
}