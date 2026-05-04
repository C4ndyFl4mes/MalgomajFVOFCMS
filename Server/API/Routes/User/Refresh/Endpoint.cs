using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FastEndpoints;
using Server.API.Data;
using Server.API.Exceptions;

namespace Server.API.Routes.User.Refresh;

public class RefreshEndpoint(AppDbContext ctx, IConfiguration config) : EndpointWithoutRequest<RefreshResponse>
{
    public override void Configure()
    {
        Post("/api/user/refresh");
        AllowAnonymous();
    }

    public override async Task<RefreshResponse> ExecuteAsync(CancellationToken ct)
    {
        Token reqToken = new()
        {
            AccessToken = HttpContext.Request.Cookies["accessToken"],
            RefreshToken = HttpContext.Request.Cookies["refreshToken"]
        };

        if (reqToken.IsEmpty)
            throw new UnauthorizedException("Tokens saknas.");

        JwtSecurityTokenHandler handler = new();
        if (!handler.CanReadToken(reqToken.AccessToken))
            throw new UnauthorizedException("Tillgångstoken kunde inte läsas.");

        JwtSecurityToken jwt = handler.ReadJwtToken(reqToken.AccessToken);

        Claim? userIdClaim = jwt.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type == "nameid"
        );
        if (!Guid.TryParse(userIdClaim?.Value, out Guid userId))
            throw new UnauthorizedException("Ogiltigt användar-ID.");

        try
        {
            RefreshData data = new(ctx, config);
            Token resToken = await data.Refresh(
                new RefreshRequest
                {
                    UserId = userId,
                    RefreshToken = reqToken.RefreshToken!
                }, ct);

            if (resToken.IsEmpty)
                throw new UnauthorizedException("Token kunde inte uppdateras.");

            HttpContext.Response.Cookies.Append("accessToken", resToken.AccessToken!,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                }
            );

            HttpContext.Response.Cookies.Append("refreshToken", resToken.RefreshToken!,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                }
            );

            return new RefreshResponse
            {
                Message = "Token uppdaterad."
            };
        }
        catch (UnauthorizedException)
        {
            HttpContext.Response.Cookies.Delete("accessToken");
            HttpContext.Response.Cookies.Delete("refreshToken");
            throw new UnauthorizedException("Ogiltig eller utgången token. Vänligen logga in igen.");
        }
    }
}