using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.BoardMember.GET;

public sealed class GetBoardMemberData(AppDbContext ctx)
{
    public async Task<IEnumerable<BoardMemberModel>> GetAllBoardMembersAsync(CancellationToken ct)
    {
        IEnumerable<BoardMemberModel> boardMembers = await ctx.BoardMembers
            .Include(bm => bm.Translations)
            .OrderBy(bm => bm.SortOrder)
            .ToListAsync(ct);

        if (!boardMembers.Any())
        {
            throw new KeyNotFoundException("Inga styrelsemedlemmar hittades.");
        }
        
        return boardMembers;
    }
}