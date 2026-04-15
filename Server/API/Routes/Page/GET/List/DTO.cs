namespace Server.API.Routes.Page.GET.List;

public record GetPageListResponse
{
    public required List<PageItemDTO> Pages { get; set; }
}

public record PageItemDTO
{
    public required Guid PageId { get; set; }
    public required string Title { get; set; }
    public required bool IsPublished { get; set; }
    public required DateTime SavedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}