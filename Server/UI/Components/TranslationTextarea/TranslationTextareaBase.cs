using Microsoft.AspNetCore.Components;

namespace Server.UI.Components.TranslationTextarea;

public class TranslationTextareaBase : ComponentBase
{
    [Parameter]
    public required TranslationFieldModel Field { get; set; } = default!;
    [Parameter]
    public EventCallback<TranslationFieldModel> OnChangeField { get; set; }
    [Parameter]
    public EventCallback<TranslationFieldModel> OnRemoveField { get; set; }

    protected Guid LanguageCodeFieldId { get; set; } = Guid.NewGuid();
    protected Guid TextFieldId { get; set; } = Guid.NewGuid();

    protected bool ProtectedLanguageCode => Field.OldLanguageCode == "sv"; // Skyddar den svenska språkkoden pga den är default.

    protected string RoundedClass => ProtectedLanguageCode ? "rounded" : "rounded-l";
}