using Microsoft.AspNetCore.Identity;

namespace Server.API.Models;

// Administrator and Editor.
public sealed class RoleModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
}