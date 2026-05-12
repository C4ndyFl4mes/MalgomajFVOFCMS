using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Server.API.Exceptions;
using Server.API.Routes.Menu.GET.State;
using Server.API.Routes.Menu.POST;
using Server.UI.Components;
using Server.UI.Layout;

namespace Server.UI.Pages.ContentPage.MenuPage;

public class MenuBase : ComponentBase, IDisposable, IAsyncDisposable
{
    [Inject]
    protected NavigationState NavigationState { get; set; } = default!;
    [Inject]
    protected GetMenuStateData GetMenuStateData { get; set; } = default!;
    [Inject]
    protected PostMenuData PostMenuData { get; set; } = default!;
    [Inject]
    protected IValidator<PostMenuRequest> PostMenuValidator { get; set; } = default!;
    [Inject]
    protected IJSRuntime JS { get; set; } = default!;
    protected GetMenuStateResponse? MenuState { get; set; }
    protected Dictionary<string, string[]> ValidationErrors { get; set; } = [];
    protected (bool IsSuccess, string Message) ResultMessage { get; set; }

    private CancellationTokenSource? _getMenuCts;
    private CancellationTokenSource? _postMenuCts;

    private bool _menuEditorNeedsInit = true;
    private IJSObjectReference? _treeModule;
    private DotNetObjectReference<MenuBase>? _selfRef;
    private const string MenuEditorRootId = "menu-editor-root";
    private const int MaxMenuDepth = 3;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

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
                Title = "Meny",
                Href = "/admin/content/menu"
            }
        ]);

        if (GetMenuStateData is not null)
            await LoadMenuStateAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (MenuState is null || !_menuEditorNeedsInit)
            return;


        _treeModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "/js/treeviewer.bundle.js");
        _selfRef ??= DotNetObjectReference.Create(this);

        await _treeModule.InvokeVoidAsync("initMenuEditor", MenuEditorRootId, _selfRef, new { maxDepth = MaxMenuDepth });
        _menuEditorNeedsInit = false;
    }

    // Laddar in menyn.
    protected async Task LoadMenuStateAsync()
    {
        CancellationTokenSource nextCts = new();
        CancellationTokenSource? previousCts = Interlocked.Exchange(ref _getMenuCts, nextCts);
        previousCts?.Cancel();
        previousCts?.Dispose();

        try
        {
            MenuState = await GetMenuStateData.GetMenuStateAsync(nextCts.Token);
            _menuEditorNeedsInit = true;
            if (nextCts.Token.IsCancellationRequested)
                return;
        }
        catch (OperationCanceledException)
        {
            // Förväntat beteende.
        }
        catch (Exception ex)
        {
            ResultMessage = (false, $"Kunde inte läsa in menyn: {ex.Message}");
        }
        finally
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    // Sparar menyn.
    protected async Task SaveMenuStateAsync()
    {
        if (MenuState is null)
        {
            ResultMessage = (false, "Kunde inte spara menyn: Menyn finns inte.");
            return;
        }

        PostMenuRequest request = new()
        {
            MenuItems = MapToPostMenuItems(MenuState.InMenu).ToList()
        };

        ValidationResult validationResult = PostMenuValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            ErrorsToDictionary(validationResult);
            await InvokeAsync(StateHasChanged);
            return;
        }

        CancellationTokenSource nextCts = new();
        CancellationTokenSource? previousCts = Interlocked.Exchange(ref _postMenuCts, nextCts);
        previousCts?.Cancel();
        previousCts?.Dispose();

        try
        {
            PostMenuResponse response = await PostMenuData.PostMenuAsync(request, nextCts.Token);
            ResultMessage = (true, response.Message);
            _menuEditorNeedsInit = true;

            if (nextCts.Token.IsCancellationRequested)
                return;
        }
        catch (OperationCanceledException)
        {
            // Förväntat beteende.
        }
        catch (KeyNotFoundException ex)
        {
            ResultMessage = (false, ex.Message);
        }
        catch (BadRequestException ex)
        {
            ResultMessage = (false, ex.Message);
        }
        catch (Exception ex)
        {
            ResultMessage = (false, $"Kunde inte spara menyn: {ex.Message}");
        }
        finally
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    [JSInvokable]
    public Task OnMenuChanged(string treeJson)
    {
        if (MenuState is null)
            return Task.CompletedTask;

        List<MenuTreeNode>? nodes = JsonSerializer.Deserialize<List<MenuTreeNode>>(treeJson, _jsonOptions);
        if (nodes is null)
            return Task.CompletedTask;

        List<MenuItemDTO> existingInMenu = Flatten(MenuState.InMenu).ToList();

        Dictionary<Guid, MenuItemDTO> existingByPageId = existingInMenu
            .GroupBy(x => x.PageInfo.PageId)
            .ToDictionary(g => g.Key, g => g.First());

        Dictionary<Guid, SimplePageInformationDTO> allPagesByPageId = MenuState.NotInMenu
            .Concat(existingInMenu.Select(x => x.PageInfo))
            .GroupBy(x => x.PageId)
            .ToDictionary(g => g.Key, g => g.First());

        MenuState.InMenu = RebuildInMenu(nodes, existingByPageId, allPagesByPageId, depth: 1);

        HashSet<Guid> usedPageIds = Flatten(MenuState.InMenu)
            .Select(x => x.PageInfo.PageId)
            .ToHashSet();

        MenuState.NotInMenu = allPagesByPageId.Values
            .Where(x => !usedPageIds.Contains(x.PageId))
            .OrderBy(x => x.Title)
            .ToList();

        return Task.CompletedTask;
    }

    private void ErrorsToDictionary(ValidationResult validationResult)
    {
        ValidationErrors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
    }

    // Bygger upp menyn baserat på trädet från JS och befintliga data.
    private List<MenuItemDTO> RebuildInMenu(
        IEnumerable<MenuTreeNode> nodes,
        IDictionary<Guid, MenuItemDTO> existingByPageId,
        IDictionary<Guid, SimplePageInformationDTO> allPagesByPageId,
        int depth)
    {
        if (depth > MaxMenuDepth)
            return [];

        List<MenuItemDTO> rebuilt = [];
        int sortOrder = 0;

        foreach (MenuTreeNode node in nodes)
        {
            if (!Guid.TryParse(node.Id, out Guid pageId))
                continue;

            if (!allPagesByPageId.TryGetValue(pageId, out SimplePageInformationDTO? pageInfo))
                continue;

            MenuItemDTO? existing = null;

            if (existingByPageId.TryGetValue(pageId, out MenuItemDTO? dto))
                existing = dto;


            Guid menuItemId = existing?.MenuItemId ??
                (Guid.TryParse(node.MenuItemId, out Guid parsed) && parsed != Guid.Empty ? parsed : Guid.NewGuid());


            MenuItemDTO menuItem = new MenuItemDTO
            {
                MenuItemId = menuItemId,
                IconId = existing?.IconId,
                SortOrder = sortOrder++,
                CustomUrl = existing?.CustomUrl,
                PageInfo = pageInfo,
                Children = RebuildInMenu(node.Children, existingByPageId, allPagesByPageId, depth + 1)
            };

            rebuilt.Add(menuItem);
        }

        return rebuilt;
    }

    private static IEnumerable<PostMenuItemDTO> MapToPostMenuItems(IEnumerable<MenuItemDTO> items)
    {
        int sortOrder = 0;

        foreach (MenuItemDTO item in items)
        {
            yield return new PostMenuItemDTO
            {
                MenuItemId = item.MenuItemId,
                PageId = item.PageInfo.PageId,
                IconId = item.IconId,
                SortOrder = sortOrder++,
                CustomUrl = item.CustomUrl,
                Children = MapToPostMenuItems(item.Children).ToList()
            };
        }
    }

    private static IEnumerable<MenuItemDTO> Flatten(IEnumerable<MenuItemDTO> items)
    {
        foreach (MenuItemDTO item in items)
        {
            yield return item;

            foreach (MenuItemDTO child in Flatten(item.Children))
            {
                yield return child;
            }
        }
    }

    public void Dispose()
    {
        CancellationTokenSource? previousGetMenuCts = Interlocked.Exchange(ref _getMenuCts, null);
        previousGetMenuCts?.Cancel();
        previousGetMenuCts?.Dispose();

        CancellationTokenSource? previousPostMenuCts = Interlocked.Exchange(ref _postMenuCts, null);
        previousPostMenuCts?.Cancel();
        previousPostMenuCts?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_treeModule is not null)
        {
            try
            {
                await _treeModule.InvokeVoidAsync("destroyMenuEditor", MenuEditorRootId);
                await _treeModule.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Förväntat beteende.
            }
        }

        _selfRef?.Dispose();
    }
}
