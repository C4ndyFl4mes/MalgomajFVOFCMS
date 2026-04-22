using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.BoardMember.POST;

public class PostBoardMemberData(AppDbContext ctx)
{
    public async Task<BoardMemberModel> PostBoardMemberAsync(BoardMemberModel boardMember, CancellationToken ct)
    {
        ctx.BoardMembers.Add(boardMember);
        await ctx.SaveChangesAsync(ct);
        return boardMember;
    }
}