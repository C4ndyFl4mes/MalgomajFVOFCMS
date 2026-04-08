using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.BoardMember.PUT;

public sealed class PutBoardMemberData(AppDbContext ctx)
{
    public async Task<BoardMemberModel> PutBoardMemberAsync(BoardMemberModel boardMember, CancellationToken ct)
    {
        BoardMemberModel existingBoardMember = await ctx.BoardMembers
            .Include(bm => bm.Translations)
            .FirstOrDefaultAsync(m => m.Id == boardMember.Id, ct) ??
                throw new KeyNotFoundException($"Styrelsemedlem med ID {boardMember.Id} kunde inte hittas.");
        
        existingBoardMember.SortOrder = boardMember.SortOrder;

        // Update translations
        existingBoardMember.Translations.Clear();
        foreach (BoardMemberTranslationModel translation in boardMember.Translations)
        {
            existingBoardMember.Translations.Add(new BoardMemberTranslationModel
            {
               LanguageCode = translation.LanguageCode,
               Text = translation.Text,
               BoardMember = existingBoardMember 
            });
        }

        await ctx.SaveChangesAsync(ct);
        return existingBoardMember;
    }
}