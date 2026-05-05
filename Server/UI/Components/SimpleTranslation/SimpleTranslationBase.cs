using Microsoft.AspNetCore.Components;

namespace Server.UI.Components.SimpleTranslation;

public class SimpleTranslationBase : ComponentBase
{
    [Parameter]
    public Dictionary<string, string> Translations { get; set; } = [];

    [Parameter]
    public EventCallback<Dictionary<string, string>> TranslationsChanged { get; set; }

    protected List<TranslationFieldModel> Fields { get; set; } = [];

    protected override void OnParametersSet()
    {
        ConvertTranslationsToFields();
    }

    protected void ConvertTranslationsToFields()
    {
        Fields = Translations.Select(t => new TranslationFieldModel
        {
            LanguageCode = t.Key,
            Text = t.Value,
            OldLanguageCode = t.Key
        }).ToList();
    }

    protected async Task HandleFieldChange(TranslationFieldModel field)
    {
        if (field.OldLanguageCode != field.LanguageCode)
        {
            Translations.Remove(field.OldLanguageCode);
        }
        Translations[field.LanguageCode] = field.Text;
        await TranslationsChanged.InvokeAsync(Translations);
    }

    protected void AddTranslation()
    {
        Fields.Add(new TranslationFieldModel
        {
            LanguageCode = string.Empty,
            Text = string.Empty,
            OldLanguageCode = string.Empty
        });
    }

    protected void RemoveTranslation(TranslationFieldModel field)
    {
        Fields.Remove(field);
        if (Translations.ContainsKey(field.OldLanguageCode))
        {
            Translations.Remove(field.OldLanguageCode);
        }
    }
}