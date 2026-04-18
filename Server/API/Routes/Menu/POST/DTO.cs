using FastEndpoints;
using FluentValidation;

namespace Server.API.Routes.Menu.POST;

public record PostMenuRequest
{
    public required List<PostMenuItemDTO> MenuItems { get; set; }
}

public record PostMenuItemDTO
{
    public required Guid MenuItemId { get; set; }
    public required Guid PageId { get; set; }
    public Guid? IconId { get; set; }
    public required int SortOrder { get; set; }
    public string? CustomUrl { get; set; }
    public List<PostMenuItemDTO> Children { get; set; } = []; // Children instead of ParenId to match the menu tree from SortableJS.
}

public record PostMenuResponse
{
    public required string Message { get; set; }
}

public class PostMenuRequestValidator : Validator<PostMenuRequest>
{
    public PostMenuRequestValidator()
    {
        RuleFor(x => x.MenuItems)
            .Must(EnsureOrderIsUnique)
            .WithMessage("Ordningen måste vara unik.");

        RuleForEach(x => x.MenuItems).ChildRules(parent =>
        {
            parent.RuleFor(x => x.Children)
                .Must(EnsureOrderIsUnique)
                .WithMessage("Ordningen för undermenyalternativ måste vara unik.");
        });
    }

    private bool EnsureOrderIsUnique(List<PostMenuItemDTO> menuItems)
    {
        List<int> sortOrders = menuItems.Select(s => s.SortOrder).ToList();
        return sortOrders.Distinct().Count() == sortOrders.Count;
    }
}