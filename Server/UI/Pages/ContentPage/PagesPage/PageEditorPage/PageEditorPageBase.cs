using Microsoft.AspNetCore.Components;
using Server.API.Enums;
using Server.API.Models;
using Server.API.Routes.ImageFile.GET;
using Server.API.Routes.Page.GET.Editor;
using Server.UI.Components;
using Server.UI.Layout;
using Ganss.Xss;
using Server.API.Routes.Page.POST;

namespace Server.UI.Pages.ContentPage.PagesPage.PageEditorPage;

public class PageEditorPageBase : ComponentBase, IDisposable
{
    [Inject]
    protected NavigationState NavigationState { get; set; } = default!;
    [Inject]
    protected GetPageEditorData GetPageEditorData { get; set; } = default!;
    [Inject]
    protected PostPageData PostPageData { get; set; } = default!;

    [Parameter]
    public Guid PageId { get; set; }

    protected PageEditorModel? PageEditorModel { get; set; }

    protected Dictionary<string, string[]> ValidationErrors { get; set; } = [];
    protected GetImagesResponse? ResponseCache { get; set; } = null; // Cache för att undvika onödiga API-anrop.
    protected HtmlSanitizer HtmlSanitizer { get; set; } = new();

    // Debounce- och versionshantering för att optimera sparandet av sidan när flera förändringar sker i snabb följd.
    private static readonly TimeSpan SaveDebounceDelay = TimeSpan.FromSeconds(2);
    private CancellationTokenSource? DebounceCts = null;
    private readonly SemaphoreSlim SaveLock = new(1, 1); // Lås för att säkerställa att endast en save-operation sker åt gången.
    private int ChangeVersion = 0;
    private int SavedVersion = 0;
    private bool Disposed = false;

    protected override async Task OnInitializedAsync()
    {
        NavigationState.SetBreadcrumbs([
            new BreadcrumbModel
            {
                Title = "Panel",
                Href = "/admin"
            },
            new BreadcrumbModel
            {
                Title = "Innehåll",
                Href = "/admin/content"
            },
            new BreadcrumbModel
            {
                Title = "Sidor",
                Href = "/admin/content/pages"
            },
            new BreadcrumbModel
            {
                Title = "Redigera",
                Href = $"/admin/content/pages/edit/{PageId}"
            }
        ]);

        if (PageEditorModel is null)
        {
            await InitializePageEditorAsync();
        }
    }

    // Initialiserar editorn genom att hämta sidans data från API:t. Om sidan inte finns (KeyNotFoundException) skapas en ny PageEditorModel med standardvärden.
    protected async Task InitializePageEditorAsync()
    {
        try
        {
            PageModel page = await GetPageEditorData.GetPageEditorAsync(new GetPageEditorRequest
            {
                PageId = PageId
            }, CancellationToken.None);

            PageEditorModel = new()
            {
                Id = page.Id,
                Meta = new PageMetaModel
                {
                    Id = page.Id,
                    IsPublished = page.IsPublished,
                    PublishedAt = page.PublishedAt,
                    SavedAt = page.SavedAt,
                    UpdatedAt = page.UpdatedAt,
                    Type = page.Type,
                    Title = page.Translations.ToDictionary(t => t.LanguageCode, t => t.Title),
                    Description = page.Translations.ToDictionary(t => t.LanguageCode, t => t.MetaDescription),
                    Keywords = page.Translations.ToDictionary(t => t.LanguageCode, t => t.MetaKeywords),
                    Slug = page.Translations.ToDictionary(t => t.LanguageCode, t => t.Slug)
                },
                Content = page.Translations.ToDictionary(t => t.LanguageCode, t => t.Content ?? string.Empty)
            };
        }
        catch (KeyNotFoundException)
        {
            PageEditorModel = new()
            {
                Id = PageId,
                Meta = new PageMetaModel()
                {
                    Id = PageId,
                    Title = new Dictionary<string, string>()
                    {
                        ["sv"] = "Namnlös sida"
                    },
                    Slug = new Dictionary<string, string>()
                    {
                        ["sv"] = "namnlos-sida"
                    },
                    Description = new Dictionary<string, string>()
                    {
                        ["sv"] = string.Empty
                    },
                    Keywords = new Dictionary<string, string>()
                    {
                        ["sv"] = string.Empty
                    },
                    Type = PageType.Page,
                    IsPublished = false,
                    SavedAt = DateTime.UtcNow,
                    PublishedAt = null,
                    UpdatedAt = null
                },
                Content = new Dictionary<string, string>()
                {
                    ["sv"] = string.Empty
                }
            };
        }
        catch (Exception ex)
        {
            ValidationErrors = new Dictionary<string, string[]>
            {
                ["init"] = [$"Det gick inte att ladda sidan: {ex.Message}"]
            };
        }
    }

    protected async Task OnMetaChangedAsync()
    {
        QueueSave();
        return;
    }

    protected async Task OnContentChangedAsync(string languageCode, string content)
    {
        if (PageEditorModel is null) return;

        HtmlSanitizer.AllowedAttributes.Add("class"); // Eftersom Quill lägger till klasser.
        HtmlSanitizer.Sanitize(content); // Vi sanerar innehållet för att undvika XSS-attacker.

        PageEditorModel.Content[languageCode] = content;
        QueueSave();
        return;
    }

    protected async Task OnContentTranslationsChangedAsync(Dictionary<string, string> contentByLanguage)
    {
        if (PageEditorModel is null) return;

        HtmlSanitizer.AllowedAttributes.Add("class");

        Dictionary<string, string> sanitizedContent = contentByLanguage.ToDictionary(
            kvp => kvp.Key,
            kvp => HtmlSanitizer.Sanitize(kvp.Value ?? string.Empty)
        );

        IEnumerable<string> removedLanguages = PageEditorModel.Content.Keys.Except(sanitizedContent.Keys);

        foreach (string language in removedLanguages)
        {
            if (language == "sv") continue; // Vi behåller svenska som standard även om det är tomt.
            PageEditorModel.Meta.Title.Remove(language);
            PageEditorModel.Meta.Description.Remove(language);
            PageEditorModel.Meta.Keywords.Remove(language);
            PageEditorModel.Meta.Slug.Remove(language);
            PageEditorModel.Content.Remove(language);
        }

        PageEditorModel.Content = sanitizedContent;
        QueueSave();
        return;
    }

    // När metadata eller innehåll förändras, antingen genom PageMeta-komponenten eller TranslationTabs-komponenten,
    // så köas en sparning av sidan. Om flera förändringar sker inom en kort tidsperiod (2 sekunder)
    // så kommer endast den senaste att sparas, vilket minskar onödiga API-anrop och förbättrar prestandan.
    private void QueueSave()
    {
        Interlocked.Increment(ref ChangeVersion);

        CancellationTokenSource next = new();
        CancellationTokenSource? previous = Interlocked.Exchange(ref DebounceCts, next);
        previous?.Cancel();
        previous?.Dispose();

        _ = DebounceThenSaveAsync(next.Token);
    }

    // När debounce-tiden har gått utan att nya förändringar sker, så sparas den senaste versionen av sidan.
    private async Task DebounceThenSaveAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(SaveDebounceDelay, token);
            await SaveLatestAsync(token);
        }
        catch (OperationCanceledException)
        {
            // Förväntat beteende, gör inget.
        }
    }

    // Sparar den senaste versionen av sidan.
    protected async Task SaveLatestAsync(CancellationToken token)
    {
        if (PageEditorModel is null || Disposed) return;

        await SaveLock.WaitAsync(token);

        try
        {
            int targetVersion = Volatile.Read(ref ChangeVersion);
            if (targetVersion <= Volatile.Read(ref SavedVersion)) return;

            PostPageRequest request = BuildRequest(PageEditorModel);
            PageModel saved = await PostPageData.SavePageAsync(request, token);

            PageEditorModel.Meta.SavedAt = saved.SavedAt;
            PageEditorModel.Meta.PublishedAt = saved.PublishedAt;
            PageEditorModel.Meta.UpdatedAt = saved.UpdatedAt;

            Volatile.Write(ref SavedVersion, targetVersion);
            ValidationErrors.Remove("save");
        }
        catch (Exception ex)
        {
            ValidationErrors["save"] = new[] { $"Det gick inte att spara sidan: {ex.Message}" };
        }
        finally
        {
            SaveLock.Release();
        }

        if (Volatile.Read(ref ChangeVersion) > Volatile.Read(ref SavedVersion))
        {
            await SaveLatestAsync(token);
        }
    }

    private static PostPageRequest BuildRequest(PageEditorModel model)
    {
        HashSet<string> languages = model.Content.Keys.ToHashSet();

        Dictionary<string, PostTranslationContentPageDTO> translations = new();
        foreach (string language in languages)
        {
            translations[language] = new PostTranslationContentPageDTO
            {
                Title = model.Meta.Title.ContainsKey(language) ? model.Meta.Title[language] : string.Empty,
                MetaDescription = model.Meta.Description.ContainsKey(language) ? model.Meta.Description[language] : string.Empty,
                MetaKeywords = model.Meta.Keywords.ContainsKey(language) ? model.Meta.Keywords[language] : string.Empty,
                Slug = model.Meta.Slug.ContainsKey(language) ? model.Meta.Slug[language] : string.Empty,
                Content = model.Content.ContainsKey(language) ? model.Content[language] : string.Empty
            };
        }

        return new PostPageRequest
        {
            Id = model.Id,
            Type = model.Meta.Type.ToString(),
            IsPublished = model.Meta.IsPublished,
            SavedAt = model.Meta.SavedAt,
            Translations = translations
        };
    }

    public void Dispose()
    {
        Disposed = true;
        DebounceCts?.Cancel();
        DebounceCts?.Dispose();
        SaveLock.Dispose();

        PageEditorModel = null;
        ResponseCache = null;
        ValidationErrors.Clear();
    }
}
