using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.BoardMember.PUT;

public class PutBoardMemberEndpoint(AppDbContext ctx) : Endpoint<PutBoardMemberRequest, PutBoardMemberResponse>
{
    public override void Configure()
    {
        Put("/api/boardmembers");
        Roles("Administrator", "Editor");
    }

    public override async Task<PutBoardMemberResponse> ExecuteAsync(PutBoardMemberRequest request, CancellationToken ct)
    {
        BoardMemberModel boardMember = PutBoardMemberMapper.MapToBoardMemberModel(request);

        PutBoardMemberData data = new(ctx);

        BoardMemberModel updatedBoardMember = await data.PutBoardMemberAsync(boardMember, ct);

        return PutBoardMemberMapper.MapToPutBoardMemberResponse(updatedBoardMember);
    }
}