
using Microsoft.AspNetCore.Identity;

namespace Server.API.Models;

public sealed class UserModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // This is a required one-to-many relationship.'
    public Guid RoleId { get; set; } // Foreign key to RoleModel
    public required RoleModel Role { get; set; } // Navigation property to RoleModel
}