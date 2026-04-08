using Server.API.Models;

namespace Server.API.Routes.BoardMember.GET;

public static class GetBoardMemberMapper
{
    public static GetBoardMemberResponse MapToBoardMemberResponse(IEnumerable<BoardMemberModel> boardMembers, string language)
    {
        return new GetBoardMemberResponse
        {
            BoardMembers = boardMembers.Select(bm => new BoardMemberDTO
            {
                Id = bm.Id,
                SortOrder = bm.SortOrder,
                Text = bm.Translations.FirstOrDefault(t => t.LanguageCode == language)?.Text ?? string.Empty 
            }).ToList()
        };
    }
}