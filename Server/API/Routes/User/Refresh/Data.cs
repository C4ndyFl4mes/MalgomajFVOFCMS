using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.API.Data;
using Server.API.Exceptions;
using Server.API.Models;

namespace Server.API.Routes.User.Refresh;

public class RefreshData(AppDbContext ctx, IConfiguration configuration)
{
    public async Task<Token> Refresh(RefreshRequest request, CancellationToken ct)
    {
        UserModel user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken, ct) ?? throw new UnauthorizedException("Ogiltig refresh token.");

        return new Token
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user, ct)
        };
    }

    private async Task<UserModel?> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken ct)
    {
        UserModel? user = await ctx.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return null;
        return user;
    }


    private string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(UserModel user, CancellationToken ct)
    {
        string refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await ctx.SaveChangesAsync(ct);
        return refreshToken;
    }

    private string CreateToken(UserModel user)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name?.ToString() ?? throw new InvalidOperationException("User role is not configured."))
        ];

        string secretKey = configuration["secret_key.txt"] ??
            throw new InvalidOperationException("Secret key is not configured.");
        string issuer = configuration["issuer.txt"] ??
            throw new InvalidOperationException("Issuer is not configured.");
        string audience = configuration["audience.txt"] ??
            throw new InvalidOperationException("Audience is not configured.");

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));

        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512);

        JwtSecurityToken securityToken = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        JwtSecurityTokenHandler tokenHandler = new();
        string token = tokenHandler.WriteToken(securityToken);

        return token;
    }
}