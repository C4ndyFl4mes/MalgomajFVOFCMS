using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Exceptions;
using Server.API.Models;

namespace Server.API.Routes.User.SignUp;

public class SignUpData(AppDbContext ctx)
{
    public async Task<SignUpResponse> SignUp(SignUpRequest request, CancellationToken ct)
    {
        if (await ctx.Users.AnyAsync(u => u.Email == request.Email, ct))
            throw new BadRequestException("En användare med den e-postadressen finns redan.");


        RoleModel role = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == "Editor", ct) ??
            throw new InvalidOperationException("Standardrollen 'Editor' är inte konfigurerad i databasen.");

        UserModel newUser = new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = new PasswordHasher<UserModel>().HashPassword(null!, request.Password),
            RoleId = role.Id,
            Role = role
        };
        
        ctx.Users.Add(newUser);
        await ctx.SaveChangesAsync(ct);

        return new SignUpResponse
        {
            Message = "Användaren har skapats."
        };
    }
}