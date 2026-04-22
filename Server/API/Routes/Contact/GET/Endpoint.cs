using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Contact.GET;

public class GetContactEndpoint(AppDbContext ctx) : EndpointWithoutRequest<GetContactResponse>
{
    public override void Configure()
    {
        Get("/api/contact");
        AllowAnonymous();
    }

    public override async Task<GetContactResponse> ExecuteAsync(CancellationToken ct)
    {
        GetContactData data = new(ctx);

        ContactModel contact = await data.GetContactAsync(ct);

        return ContactGetMapper.MapToGetContactResponse(contact);
    }

}