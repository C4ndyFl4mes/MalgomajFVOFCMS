using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Server.API.Models;
using Server.API.Routes.Page.GET.List;

namespace Server.UI.Components.PageMeta;

public class PageMetaBase : ComponentBase
{
    [Inject]
    protected GetPageListData GetPageListData { get; set; } = default!;

    [Parameter]
    public required PageMetaModel Meta { get; set; } = default!;
    [Parameter]
    public EventCallback<PageMetaModel> MetaChanged { get; set; }

    protected Dictionary<string, string[]> ValidationErrors { get; set; } = [];

    protected async Task HandleMetaChange()
    {
        GenerateSlug();
        await MetaChanged.InvokeAsync(Meta);
    }

    protected async Task HandlePublishToggleAsync()
    {
        try
        {
            List<PageModel> pages = await GetPageListData.GetPageListAsync(CancellationToken.None);

            if (Meta.IsPublished)
            {
                Meta.IsPublished = false;
            }
            else
            {
                // Kontrollera om det finns en annan publicerad sida i samma språk med samma slug
                bool slugConflict = pages.Any(p => p.Id != Meta.Id && p.IsPublished && p.Translations.Any(t => Meta.Slug.ContainsKey(t.LanguageCode) && Meta.Slug[t.LanguageCode] == t.Slug));
                Console.WriteLine($"Slug conflict: {slugConflict}");
                if (slugConflict)
                {
                    ValidationErrors["slug"] = ["En annan publicerad sida har samma slug. Vänligen ändra slug innan du publicerar."];
                    return;
                }
                Meta.IsPublished = true;
                ValidationErrors.Remove("slug");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett fel uppstod vid hämtning av sidlistan: {ex.Message}");
        }
    }



    private void GenerateSlug()
    {
        Dictionary<string, string> titleTranslations = Meta.Title;

        foreach ((string language, string title) in titleTranslations)
        {
            if (string.IsNullOrWhiteSpace(title)) continue;

            string slug = Slugify(title);
            Meta.Slug[language] = slug;
        }
    }

    private static string Slugify(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = input.ToLowerInvariant();

        input = RemoveDiacritics(input);

        input = Regex.Replace(input, @"[^a-z0-9\s-]", "");
        input = Regex.Replace(input, @"\s+", "-").Trim();
        input = Regex.Replace(input, @"-+", "-");

        return input;
    }

    private static string RemoveDiacritics(string text)
    {
        string normalized = text.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        foreach (char c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}