using Server.API.Models;

namespace Server.API.Routes.Contact.GET;

public static class ContactGetMapper
{
    public static GetContactResponse MapToGetContactResponse(ContactModel contact)
    {
        return new GetContactResponse
        {
            Email = contact.Email,
            PhoneNumber = contact.Phone,
            Address = contact.Address
        };
    }
}