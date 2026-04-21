using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.User.SignUp;

public class SignUpEndpoint(AppDbContext ctx) : Endpoint<SignUpRequest, SignUpResponse>
{
    public override void Configure()
    {
        Post("/api/user/signup");
        Roles("Administrator");
    }

    public override async Task<SignUpResponse> ExecuteAsync(SignUpRequest request, CancellationToken ct)
    {
        SignUpData data = new(ctx);

        return await data.SignUp(request, ct);
    }
}