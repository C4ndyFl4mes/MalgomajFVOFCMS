namespace Server.API.Models;

public sealed class MenuItemModel
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }

    // Nagivation property to the page this menu item links to.
    public Guid? PageId { get; set; }
    public PageModel? Page { get; set; }

    public required int SortOrder { get; set; }
    public string? CustomUrl { get; set; }

    // Navigation property for image icon of the menu item.
    public ImageModel? Icon { get; set; }
    public Guid? IconId { get; set; }
}