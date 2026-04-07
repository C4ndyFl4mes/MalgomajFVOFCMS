using System.ComponentModel.DataAnnotations;

namespace Server.API.Models;

public sealed class ContactModel
{
    public Guid Id { get; set; }
    public required string Address { get; set; }

    [Phone]
    public required string Phone { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
}