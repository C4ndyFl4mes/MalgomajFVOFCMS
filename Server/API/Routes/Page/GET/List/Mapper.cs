using Server.API.Models;

namespace Server.API.Routes.Page.GET.List;

public static class GetPageListMapper
{
    public static GetPageListResponse ToResponse(List<PageModel> models)
    {
        return new GetPageListResponse
        {
            Pages = models.Select(model => new PageItemDTO
            {
                PageId = model.Id,
                Title = model.Translations.FirstOrDefault(t => t.LanguageCode == "sv")?.Title ?? string.Empty,
                IsPublished = model.IsPublished,
                SavedAt = model.SavedAt,
                PublishedAt = model.PublishedAt,
                UpdatedAt = model.UpdatedAt
            }).ToList()
        };
    }
}