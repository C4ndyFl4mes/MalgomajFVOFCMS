using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.BoardMember.GET;

public class GetBoardMemberEndpoint(AppDbContext ctx) : Endpoint<GetBoardMemberRequest, GetBoardMemberResponse>
{
    public override void Configure()
    {
        Get("/api/boardmembers");
        AllowAnonymous();
    }

    public override async Task<GetBoardMemberResponse> ExecuteAsync(GetBoardMemberRequest request, CancellationToken ct)
    {
        GetBoardMemberData data = new(ctx);

        IEnumerable<BoardMemberModel> boardMembers = await data.GetAllBoardMembersAsync(ct);

        return GetBoardMemberMapper.MapToBoardMemberResponse(boardMembers, request.LanguageCode);
    }
}