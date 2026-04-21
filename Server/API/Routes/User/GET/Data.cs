using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.User.GET;

public class GetUserData(AppDbContext ctx)
{
    public async Task<List<UserModel>> GetUsersAsync(GetUserRequest request, CancellationToken ct)
    {
        if (request.UserId.HasValue)
        {
            UserModel? user = await ctx.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == request.UserId.Value, ct) ??
                throw new KeyNotFoundException("Användaren hittades inte.");
            return [user];
        }
        else
        {
            return await ctx.Users.Include(u => u.Role).ToListAsync(ct);
        }
    }
}