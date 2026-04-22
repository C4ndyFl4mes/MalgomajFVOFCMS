using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Contact.GET;

public class GetContactData(AppDbContext ctx)
{
    public async Task<ContactModel> GetContactAsync(CancellationToken ct)
    {
        ContactModel contact = await ctx.Contact.FirstOrDefaultAsync(ct) ??
            throw new InvalidOperationException("Kontaktinformation saknas i databasen.");

        return contact;
    }
}