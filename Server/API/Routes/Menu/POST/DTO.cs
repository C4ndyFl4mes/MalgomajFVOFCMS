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
    private const int MaxDepth = 3;
    public PostMenuRequestValidator()
    {
        RuleFor(x => x.MenuItems)
            .Must(HaveUniqueSortOrdersAtEveryLevel)
            .WithMessage("Ordningen måste vara unik för varje nivå i menyn.");

        RuleFor(x => x.MenuItems)
            .Must(items => GetMaxDepth(items) <= MaxDepth)
            .WithMessage($"Menyn får maximalt ha {MaxDepth} nivåer.");
    }

    private static bool HaveUniqueSortOrdersAtEveryLevel(List<PostMenuItemDTO> items)
    {
        if (items.GroupBy(x => x.SortOrder).Any(g => g.Count() > 1))
            return false;
        
        return items.All(item => HaveUniqueSortOrdersAtEveryLevel(item.Children));
    }

    private static int GetMaxDepth(List<PostMenuItemDTO> items, int depth = 1)
    {
        if (items.Count == 0)
            return depth - 1;
        
        return items.Max(item => GetMaxDepth(item.Children, depth + 1));
    }
}