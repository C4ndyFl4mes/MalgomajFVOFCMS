using Microsoft.AspNetCore.Components;

namespace Server.UI.Components.TranslationTabs;

public class TranslationTabsBase : ComponentBase
{
    [Parameter]
    public Dictionary<string, string> Translations { get; set; } = [];
    [Parameter]
    public EventCallback<Dictionary<string, string>> TranslationsChanged { get; set; }

    protected List<TabModel> Tabs { get; set; } = [];
    protected TabModel? ActiveTab { get; set; }

    private bool IsInitialized { get; set; } = false;

    protected override void OnParametersSet()
    {
        if (!IsInitialized)
        {
            Tabs = Translations.Select(t => new TabModel
            {
                LanguageCode = t.Key,
                OldLanguageCode = t.Key,
                Content = t.Value,
                EditorId = Guid.NewGuid()
            }).ToList();

            if (!Tabs.Any())
            {
                Tabs.Add(new TabModel
                {
                    LanguageCode = "sv",
                    OldLanguageCode = "sv",
                    Content = string.Empty,
                    EditorId = Guid.NewGuid()
                });
            }
            ActiveTab = Tabs.FirstOrDefault();
            IsInitialized = true;
            return;
        }

        foreach (TabModel tab in Tabs)
        {
            if (!string.IsNullOrWhiteSpace(tab.OldLanguageCode) && Translations.TryGetValue(tab.OldLanguageCode, out string? content))
                tab.Content = content;
        }

        foreach ((string languageCode, string content) in Translations)
        {
            bool exists = Tabs.Any(t => t.OldLanguageCode == languageCode || t.LanguageCode == languageCode);
            if (!exists)
            {
                Tabs.Add(new TabModel
                {
                    LanguageCode = languageCode,
                    OldLanguageCode = languageCode,
                    Content = content,
                    EditorId = Guid.NewGuid()
                });
            }
        }

        if (ActiveTab is null || !Tabs.Contains(ActiveTab))
            ActiveTab = Tabs.FirstOrDefault();
    }

    protected void SelectTab(TabModel tab)
    {
        ActiveTab = tab;
    }

    protected void AddTranslation()
    {
        TabModel tab = new()
        {
            LanguageCode = string.Empty,
            OldLanguageCode = string.Empty,
            Content = string.Empty,
            EditorId = Guid.NewGuid()
        };
        Tabs.Add(tab);
        ActiveTab = tab;
    }

    protected bool IsProtected(TabModel tab)
    {
        return tab.OldLanguageCode == "sv";
    }

    protected string GetTabLabel(TabModel tab)
    {
        return string.IsNullOrWhiteSpace(tab.LanguageCode) ? "Ny översättning" : tab.LanguageCode;
    }

    // När förändring sker skickas en uppdatering till föräldern.
    protected async Task OnLanguageChangedAsync(TabModel tab)
    {
        string newCode = (tab.LanguageCode ?? string.Empty).Trim().ToLowerInvariant();

        tab.LanguageCode = newCode;

        if (newCode.Length != 2) return;


        if (Tabs.Any(t => !ReferenceEquals(t, tab) && t.LanguageCode == newCode))
        {
            tab.LanguageCode = tab.OldLanguageCode;
            return;
        }

        if (!string.IsNullOrWhiteSpace(tab.OldLanguageCode))
            Translations.Remove(tab.OldLanguageCode);


        tab.OldLanguageCode = newCode;
        Translations[newCode] = tab.Content ?? string.Empty;

        await TranslationsChanged.InvokeAsync(Translations);
    }

    protected async Task OnContentChangedAsync((string editorId, string content) payload)
    {
       TabModel? targetTab = Tabs.FirstOrDefault(t => $"quill-{t.EditorId}" == payload.editorId);
        if (targetTab is null) return;

        targetTab.Content = payload.content;
        string key = targetTab.LanguageCode.Trim().ToLowerInvariant();
        targetTab.LanguageCode = key;

        if (key.Length == 2)
        {
            Translations[key] = payload.content;
            await TranslationsChanged.InvokeAsync(Translations);
        }
    }

    protected async Task RemoveTranslationsAsync(TabModel tab)
    {
        if (IsProtected(tab)) return;

        Tabs.Remove(tab);

        if (!string.IsNullOrWhiteSpace(tab.OldLanguageCode))
            Translations.Remove(tab.OldLanguageCode);

        if (!string.IsNullOrWhiteSpace(tab.LanguageCode))
            Translations.Remove(tab.LanguageCode);

        if (ReferenceEquals(ActiveTab, tab))
            ActiveTab = Tabs.FirstOrDefault();

        await TranslationsChanged.InvokeAsync(Translations);
    }

    protected string GetActiveContent()
    {
        return ActiveTab?.Content ?? string.Empty;
    }

    protected string GetActiveLanguageCode()
    {
        return ActiveTab?.LanguageCode ?? string.Empty;
    }

    protected string GetActiveEditorId()
    {
        return ActiveTab is null ? string.Empty : $"quill-{ActiveTab.EditorId}";
    }

    protected bool ActiveTabCanRenderEditor()
    {
        if (ActiveTab is null) return false;
        return !string.IsNullOrWhiteSpace(ActiveTab.LanguageCode) && ActiveTab.LanguageCode.Trim().Length == 2;
    }
}