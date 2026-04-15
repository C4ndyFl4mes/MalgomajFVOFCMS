using Server.API.Enums;
using Server.API.Exceptions;
using Server.API.Models;

namespace Server.API.Routes.Page.POST;

public static class PostPageMapper
{
    public static PageModel ToModel(PostPageRequest request, Guid pageId)
    {
        return new PageModel
        {
            Id = pageId,
            Type = Enum.TryParse(request.Type, true, out PageType type) ? type : throw new BadRequestException("Sidtypen är fel."),
            IsPublished = request.IsPublished,
            SavedAt = request.SavedAt,
            Translations = request.Translations.Select(kvp => new PageTranslationModel
            {
                PageId = pageId,
                LanguageCode = kvp.Key,
                Title = kvp.Value.Title,
                Content = kvp.Value.Content,
                Slug = kvp.Value.Slug,
                MetaDescription = kvp.Value.MetaDescription,
                MetaKeywords = kvp.Value.MetaKeywords,
                Page = null! // This will be set by EF Core when we add the PageModel to the context.
            }).ToList()
        };
    }

    public static PostPageResponse ToResponse(PageModel page)
    {
        return new PostPageResponse
        {
            Message = "Sidan har sparats.",
            IsPublished = page.IsPublished,
            SavedAt = page.SavedAt,
            PublishedAt = page.PublishedAt,
            UpdatedAt = page.UpdatedAt
        };
    }
}