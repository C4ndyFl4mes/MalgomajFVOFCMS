using AngleSharp.Text;
using Microsoft.AspNetCore.Components;
using Server.API.Models;
using Server.API.Routes.Page.GET.List;
using Server.UI.Layout;

namespace Server.UI.Pages.ContentPage.PagesPage;

public class PagesBase : ComponentBase, IDisposable
{
    [Inject]
    protected NavigationState NavigationState { get; set; } = default!;
    [Inject]
    protected GetPageListData GetPageListData { get; set; } = default!;

    protected Guid Id { get; set; } = Guid.NewGuid();
    protected string Href { get; set; } = string.Empty; // Href för att navigera till editorn, kan sättas baserat på Id eller annan logik.
    protected List<PageItemDTO> DraftPages { get; set; } = [];
    protected List<PageItemDTO> PublishedPages { get; set; } = [];
    protected Dictionary<string, string[]> Errors { get; set; } = new();

    private CancellationTokenSource? _cts = null;

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
            }
        ]);

        Href = $"/admin/content/pages/edit/{Id}";

        if (GetPageListData is not null)
            await LoadPages();
    }

    // Laddar in sidor (utkast och publicerade)
    private async Task LoadPages()
    {
        CancellationTokenSource nextCts = new();
        CancellationTokenSource? previousCts = Interlocked.Exchange(ref _cts, nextCts);
        previousCts?.Cancel();
        previousCts?.Dispose();

        try
        {
            List<PageModel> models = await GetPageListData.GetPageListAsync(nextCts.Token);
            if (nextCts.IsCancellationRequested)
                return;
            
            List<PageItemDTO> pages = GetPageListMapper.ToResponse(models).Pages;
            foreach (PageItemDTO page in pages)
            {
                if (page.IsPublished)
                {
                    PublishedPages.Add(page);
                }
                else
                {
                    DraftPages.Add(page);
                }
            }

            PublishedPages = PublishedPages.OrderByDescending(page => page.UpdatedAt).ToList();
            DraftPages = DraftPages.OrderByDescending(page => page.SavedAt).ToList();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Page list loading was canceled.");
        }
        catch (Exception ex)
        {
            Errors["loadPages"] = [$"Ett fel inträffade vid inläsning av sidor: {ex.Message}"];
        }
    }

    // Skapar ett utdrag för titel.
    protected string TitleExcerpt(string title)
    {
        string[] words = title.SplitSpaces();
        string excerpt = "";

        foreach(string word in words)
        {
            if ($"{excerpt} {word}".Count() <= 17)
            {
                excerpt = $"{excerpt} {word}";
            }
            else
            {
                excerpt = $"{excerpt}...";
                break;
            }
        }

        return excerpt;
    }

    public void Dispose()
    {
        CancellationTokenSource? cts = Interlocked.Exchange(ref _cts, null);
        cts?.Cancel();
        cts?.Dispose();
    }
}
