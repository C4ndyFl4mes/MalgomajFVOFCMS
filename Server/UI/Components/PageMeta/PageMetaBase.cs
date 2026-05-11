using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Server.API.Models;
using Server.API.Routes.Page.GET.List;

namespace Server.UI.Components.PageMeta;

public class PageMetaBase : ComponentBase, IDisposable
{
    [Inject]
    protected GetPageListData GetPageListData { get; set; } = default!;

    [Parameter]
    public required PageMetaModel Meta { get; set; } = default!;
    [Parameter]
    public EventCallback<PageMetaModel> MetaChanged { get; set; }
    [Parameter]
    public bool IsPublishing { get; set; } = false;
    [Parameter]
    public EventCallback<bool> OnPublish { get; set; }
    [Parameter]
    public bool IsUnpublishing { get; set; } = false;
    [Parameter]
    public EventCallback<bool> OnUnpublish { get; set; }


    protected Dictionary<string, string[]> ValidationErrors { get; set; } = [];
    

    private CancellationTokenSource? _cts = null;

    public async Task StopPublishing()
    {
        await HandleOnPublish();
    }

    public async Task StopRedacting()
    {
        await HandleOnUnpublish();
    }

    protected async Task HandleMetaChange()
    {
        GenerateSlug();
        await MetaChanged.InvokeAsync(Meta);
    }

    protected async Task HandleOnPublish()
    {
        IsPublishing = !IsPublishing;
        await OnPublish.InvokeAsync(IsPublishing);
    }

    protected async Task HandleOnUnpublish()
    {
        IsUnpublishing = !IsUnpublishing;
        await OnUnpublish.InvokeAsync(IsUnpublishing);
    }

    protected async Task HandlePublishToggleAsync()
    {
        if (!Meta.IsPublished)
            await HandleOnPublish();
        if (Meta.IsPublished)
            await HandleOnUnpublish();

        CancellationTokenSource nextCts = new();
        CancellationTokenSource? previousCts = Interlocked.Exchange(ref _cts, nextCts);
        previousCts?.Cancel();
        previousCts?.Dispose();

        try
        {
            List<PageModel> pages = await GetPageListData.GetPageListAsync(CancellationToken.None);

            if (nextCts.IsCancellationRequested)
            {
                if (!Meta.IsPublished)
                    await HandleOnPublish();
                if (Meta.IsPublished)
                    await HandleOnUnpublish();
                    
                return;
            }

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
            await HandleMetaChange();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Page list loading was canceled.");
        }
        catch (Exception ex)
        {
            ValidationErrors["pageList"] = [$"Kunde inte läsa in alla sidor: {ex.Message}"];
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

    public void Dispose()
    {
        CancellationTokenSource? cts = Interlocked.Exchange(ref _cts, null);
        cts?.Cancel();
        cts?.Dispose();
    }
}