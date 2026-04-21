using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.User.DELETE;

public class DeleteUserEndpoint(AppDbContext ctx) : Endpoint<DeleteUserRequest, DeleteUserResponse>
{
    public override void Configure()
    {
        Delete("/api/user/{UserId}");
        Roles("Administrator");
    }

    public override async Task<DeleteUserResponse> ExecuteAsync(DeleteUserRequest request, CancellationToken ct)
    {
        DeleteUserData data = new(ctx);

        return await data.DeleteUserAsync(request, ct);
    }
}