namespace Server.API.Routes.Contact.GET;

public record GetContactResponse
{
    public required string Email;
    public required string PhoneNumber;
    public required string Address;
}

