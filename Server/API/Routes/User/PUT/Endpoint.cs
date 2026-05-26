using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.User.PUT;

public class UpdatePasswordEndpoint(AppDbContext ctx, IConfiguration configuration) : Endpoint<UpdatePasswordRequest, UpdatePasswordResponse>
{
    public override void Configure()
    {
        Put("/api/user/change-password");
        Roles("Administrator", "Editor");
    }

    public override async Task<UpdatePasswordResponse> ExecuteAsync(UpdatePasswordRequest request, CancellationToken ct)
    {
        UpdatePasswordData data = new(ctx, configuration);

        Token token = await data.UpdatePassword(request, ct);

        HttpContext.Response.Cookies.Append("accessToken", token.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15),
            Path = "/"
        });

        HttpContext.Response.Cookies.Append("refreshToken", token.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Path = "/"
        });

        return new UpdatePasswordResponse
        {
            Message = "Lösenordet uppdaterades."
        };
    }
}