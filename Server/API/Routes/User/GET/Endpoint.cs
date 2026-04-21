using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.User.GET;

public class GetUserEndpoint(AppDbContext ctx) : Endpoint<GetUserRequest, GetUsersResponse>
{
    public override void Configure()
    {
        Get("/api/user/{UserId?}");
        Roles("Administrator");
    }

    public override async Task<GetUsersResponse> ExecuteAsync(GetUserRequest request, CancellationToken ct)
    {
        GetUserData data = new(ctx);

        List<UserModel> users = await data.GetUsersAsync(request, ct);

        return GetUserMapper.ToResponse(users);
    }
}