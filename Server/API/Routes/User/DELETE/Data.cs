using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.User.DELETE;

public class DeleteUserData(AppDbContext ctx)
{
    public async Task<DeleteUserResponse> DeleteUserAsync(DeleteUserRequest request, CancellationToken ct)
    {
        Console.WriteLine($"Attempting to delete user with ID: {request.UserId}");
        UserModel user = await ctx.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == request.UserId, ct) ??
            throw new KeyNotFoundException("Användaren hittades inte.");
        
        if (user.Role.Name == "Administrator")
            throw new InvalidOperationException("Administratörer kan inte raderas.");
        
        ctx.Users.Remove(user);

        await ctx.SaveChangesAsync(ct);

        return new DeleteUserResponse
        {
            UserId = user.Id,
            Message = "Användaren har raderats."
        };
    }
}