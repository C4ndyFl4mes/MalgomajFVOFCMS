using FastEndpoints;
using Server.API.Data;

namespace Server.API.Routes.BoardMember.DELETE;

public class DeleteBoardMemberEndpoint(AppDbContext ctx) : Endpoint<DeleteBoardMemberRequest, DeleteBoardMemberResponse>
{
    public override void Configure()
    {
        Delete("/api/boardmembers/{id}");
        Roles("Administrator", "Editor");
    }

    public override async Task<DeleteBoardMemberResponse> ExecuteAsync(DeleteBoardMemberRequest request, CancellationToken ct)
    {
        DeleteBoardMemberData data = new(ctx);

        await data.DeleteBoardMemberAsync(request.Id, ct);

        return DeleteBoardMemberMapper.MapToDeleteBoardMemberResponse(request.Id);
    }
}