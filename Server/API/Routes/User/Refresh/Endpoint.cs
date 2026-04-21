using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.User.Refresh;

public class RefreshEndpoint(AppDbContext ctx, IConfiguration configuration) : Endpoint<RefreshRequest, RefreshResponse>
{
    public override void Configure()
    {
        Post("/api/user/refresh");
        AllowAnonymous();
    }

    public override async Task<RefreshResponse> ExecuteAsync(RefreshRequest request, CancellationToken ct)
    {
        RefreshData data = new(ctx, configuration);

        Token token = await data.Refresh(request, ct);

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

        return new RefreshResponse
        {
            Message = "Token uppdaterad."
        };
    }
}