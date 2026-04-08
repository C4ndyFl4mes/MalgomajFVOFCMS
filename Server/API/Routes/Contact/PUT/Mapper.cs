using Server.API.Models;

namespace Server.API.Routes.Contact.PUT;

public static class ContactPutMapper
{
    public static PutContactResponse MapToPutContactResponse(ContactModel contact)
    {
        return new PutContactResponse
        {
            Email = contact.Email,
            PhoneNumber = contact.Phone,
            Address = contact.Address
        };
    }
}