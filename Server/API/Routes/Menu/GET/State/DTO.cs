namespace Server.API.Routes.Menu.GET.State;

public record GetMenuStateResponse
{
    public required List<SimplePageInformationDTO> NotInMenu { get; set; }
    public required List<MenuItemDTO> InMenu { get; set; }
}

public record MenuItemDTO
{
    public required Guid MenuItemId { get; set; }
    public Guid? IconId { get; set; }
    public required int SortOrder { get; set; }
    public string? CustomUrl { get; set; }
    public required SimplePageInformationDTO PageInfo { get; set; }
    public List<MenuItemDTO> Children { get; set; } = [];
}

public record SimplePageInformationDTO
{
    public required Guid PageId { get; set; }
    public required string Title { get; set; }
}