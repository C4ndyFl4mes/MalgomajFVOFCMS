using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.BoardMember.POST;

public sealed class PostBoardMemberEndpoint(AppDbContext ctx) : Endpoint<PostBoardMemberRequest, PostBoardMemberResponse>
{
    public override void Configure()
    {
        Post("/api/boardmembers");
        AllowAnonymous();
    }

    public override async Task<PostBoardMemberResponse> ExecuteAsync(PostBoardMemberRequest request, CancellationToken ct)
    {
        BoardMemberModel boardMember = PostBoardMemberMapper.MapToBoardMemberModel(request);

        PostBoardMemberData data = new(ctx);

        BoardMemberModel createdBoardMember = await data.PostBoardMemberAsync(boardMember, ct);

        return PostBoardMemberMapper.MapToPostBoardMemberResponse(createdBoardMember);
    }
}