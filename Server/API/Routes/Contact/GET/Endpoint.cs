using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Contact.GET;

public sealed class GetContactEndpoint(AppDbContext ctx) : EndpointWithoutRequest<GetContactResponse>
{
    public override void Configure()
    {
        Get("/api/contact");
        AllowAnonymous();
    }

    public override async Task<GetContactResponse> ExecuteAsync(CancellationToken ct)
    {
        ContactModel contact = await ctx.Contact.FirstOrDefaultAsync(ct) ??
            throw new InvalidOperationException("Kontaktinformation saknas i databasen.");

        return ContactGetMapper.MapToGetContactResponse(contact);
    }

}