using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Contact.PUT;

public class ContactPutData(AppDbContext ctx)
{
    public async Task<ContactModel> UpdateContactAsync(PutContactRequest request, CancellationToken ct)
    {
        ContactModel contact = await ctx.Contact.FirstOrDefaultAsync(ct) ??
            throw new InvalidOperationException("Kontaktinformationen kunde inte hittas.");
        
        contact.Email = request.Email;
        contact.Phone = request.PhoneNumber;
        contact.Address = request.Address;

        if (ctx.ChangeTracker.HasChanges())
        {
            await ctx.SaveChangesAsync(ct);
        }

        return contact;
    }
}