using Server.API.Models;

namespace Server.API.Routes.BoardMember.DELETE;

public static class DeleteBoardMemberMapper
{
    public static DeleteBoardMemberResponse MapToDeleteBoardMemberResponse(Guid id)
    {
        return new DeleteBoardMemberResponse
        {
            Id = id,
            Message = $"Styrelsemedlemmen har raderats. ID: {id}"
        };
    }
}