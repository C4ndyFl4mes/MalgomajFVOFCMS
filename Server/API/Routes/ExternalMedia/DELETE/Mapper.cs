namespace Server.API.Routes.ExternalMedia.DELETE;

public static class DeleteExternalMediaMapper
{
    public static DeleteExternalMediaResponse MapToDeleteExternalMediaResponse(Guid id)
    {
        return new DeleteExternalMediaResponse
        {
            Id = id,
            Message = $"Extern media har raderats. ID: {id}"
        };
    }
}