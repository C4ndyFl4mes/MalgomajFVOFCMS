using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Server.UI.Components.QuillEditor;

public class QuillEditorBase : ComponentBase
{
    [Inject]
    protected IJSRuntime JS { get; set; } = default!;

    [Parameter]
    public string Content { get; set; } = string.Empty;
    [Parameter]
    public EventCallback<(string, string)> ContentChanged { get; set; }
    [Parameter]
    public string LanguageCode { get; set; } = "sv";
    [Parameter]
    public required string EditorId { get; set; } 

    protected bool IsImageSelectorOpen { get; set; } = false;

    protected IJSObjectReference? Module { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/editor.bundle.js");

            await Module.InvokeVoidAsync("initQuill", EditorId, DotNetObjectReference.Create(this));

            if (!string.IsNullOrEmpty(Content))
            {
                await Module.InvokeVoidAsync("setHTMLContent", EditorId, Content);
            }
        }
    }

    [JSInvokable]
    public async Task UpdateContent(string content)
    {
        Content = content;
        await ContentChanged.InvokeAsync((EditorId, content));
    }

    [JSInvokable]
    public void OpenImageSelector()
    {
        IsImageSelectorOpen = true;
        StateHasChanged();
    }

    protected async Task HandleImageSelected(ImageInspectionModel inspectionModel)
    {
        IsImageSelectorOpen = false;
        if (Module != null)
        {
            await Module.InvokeVoidAsync("insertImage", EditorId, inspectionModel.Image.Id);
        }
        StateHasChanged();
    }
}