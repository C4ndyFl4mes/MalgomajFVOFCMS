using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.User.SignIn;

public class SignInEndpoint(AppDbContext ctx, IConfiguration configuration) : Endpoint<SignInRequest, SignInResponse>
{
    public override void Configure()
    {
        Post("/api/user/signin");
        AllowAnonymous();
    }

    public override async Task<SignInResponse> ExecuteAsync(SignInRequest request, CancellationToken ct)
    {
        SignInData data = new(ctx, configuration);

        Token token = await data.SignIn(request, ct);

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

        return new SignInResponse
        {
            Message = "Inloggning lyckades."
        };
    }
}