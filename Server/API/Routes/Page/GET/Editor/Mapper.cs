using Server.API.Enums;
using Server.API.Models;

namespace Server.API.Routes.Page.GET.Editor;

public static class GetPageEditorMapper
{
    public static GetPageEditorResponse ToResponse(PageModel model)
    {
        return new GetPageEditorResponse
        {
            PageId = model.Id,
            Type = model.Type.ToString(),
            IsPublished = model.IsPublished,
            SavedAt = model.SavedAt,
            PublishedAt = model.PublishedAt,
            UpdatedAt = model.UpdatedAt,
            Translations = model.Translations.ToDictionary(
                t => t.LanguageCode,
                t => new GetTranslationContentPageDTO
                {
                    Title = t.Title,
                    Content = t.Content,
                    Slug = t.Slug,
                    MetaDescription = t.MetaDescription,
                    MetaKeywords = t.MetaKeywords
                })
        };
    }
}