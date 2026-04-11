namespace Server.API.Routes.ImageFile.DELETE;

public static class DeleteImageMapper
{
    public static DeleteImageResponse ToResponse(Guid id)
    {
        return new DeleteImageResponse
        {
            Id = id,
            Message = $"Bild med ID {id} har tagits bort."
        };
    }
}