using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Contact.PUT;

public class ContactPutEndpoint(AppDbContext ctx) : Endpoint<PutContactRequest, PutContactResponse>
{
    public override void Configure()
    {
        Put("/api/contact");
        Roles("Administrator", "Editor");
    }

    public override async Task<PutContactResponse> ExecuteAsync(PutContactRequest request, CancellationToken ct)
    {
        ContactPutData data = new ContactPutData(ctx);
        
        ContactModel updatedContact = await data.UpdateContactAsync(request, ct);
     
        return ContactPutMapper.MapToPutContactResponse(updatedContact);
    }
}