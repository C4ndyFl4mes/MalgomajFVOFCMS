using FastEndpoints;
using FluentValidation;

namespace Server.API.Routes.Slide.POST;

public record PostSlideRequest
{
    public required List<PostSlideDTO> Slides { get; set; }
}

public record PostSlideDTO
{
    public required Guid ImageID { get; set; }
    public required int SortOrder { get; set; }
}

public record PostSlideResponse
{
    public required string Message { get; set; }
}

public class PostSlideRequestValidator : Validator<PostSlideRequest>
{
    public PostSlideRequestValidator()
    {
        RuleFor(x => x.Slides)
            .Must(EnsureOrderIsUnique)
            .WithMessage("Ordningen måste vara unik.")
            .Must(EnsureImageIdsAreUnique)
            .WithMessage("Bild-ID måste vara unikt.");
    }

    private bool EnsureOrderIsUnique(List<PostSlideDTO> slides)
    {
        List<int> sortOrders = slides.Select(s => s.SortOrder).ToList();
        return sortOrders.Distinct().Count() == sortOrders.Count;
    }

    private bool EnsureImageIdsAreUnique(List<PostSlideDTO> slides)
    {
        HashSet<Guid> imageIds = slides.Select(s => s.ImageID).ToHashSet();
        return imageIds.Count == slides.Count;
    }
}