using Microsoft.AspNetCore.Components;

namespace Server.UI.Components.Overlay;

public class OverlayBase : ComponentBase, IDisposable
{
    [Parameter]
    public bool IsOpen { get; set; } = false;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback<bool> Changed { get; set; }

    public void Dispose()
    {
        if (IsOpen)
        {
            _ = Close();
        }
    }

    protected async Task Close()
    {
        IsOpen = false;
        await Changed.InvokeAsync(false);
    }
}