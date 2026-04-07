
using Microsoft.AspNetCore.Identity;

namespace Server.API.Models;

public sealed class UserModel : IdentityUser<Guid>
{
    // Navigation property for the user's role.
    public Guid RoleId { get; set; }
    public required RoleModel Role { get; set; }
    public required string Name { get; set; }
}