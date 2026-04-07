using Microsoft.AspNetCore.Identity;

namespace Server.API.Models;

// Administrator and Editor.
public sealed class RoleModel : IdentityRole<Guid>
{
    // Description of the role.
    public string Description { get; set; } = string.Empty;
}