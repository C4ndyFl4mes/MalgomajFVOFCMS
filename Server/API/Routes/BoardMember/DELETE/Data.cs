using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.BoardMember.DELETE;

public class DeleteBoardMemberData(AppDbContext ctx)
{
    public async Task DeleteBoardMemberAsync(Guid id, CancellationToken ct)
    {
        BoardMemberModel boardMember = await ctx.BoardMembers.FindAsync(id, ct) ??
            throw new KeyNotFoundException($"Styrelsemedlemmen finns inte. ID: {id}");
        
        ctx.BoardMembers.Remove(boardMember);
        await ctx.SaveChangesAsync(ct);
    }
}