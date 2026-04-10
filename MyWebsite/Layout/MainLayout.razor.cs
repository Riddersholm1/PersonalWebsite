using Microsoft.JSInterop;

namespace MyWebsite.Layout;

public partial class MainLayout : IAsyncDisposable
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    private bool _isDarkMode = true;
    private bool _userHasToggled;
    private DotNetObjectReference<MainLayout>? _dotNetRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        _dotNetRef = DotNetObjectReference.Create(this);
        var prefersDark = await JS.InvokeAsync<bool>("themeDetector.init", _dotNetRef);

        _isDarkMode = prefersDark;
        StateHasChanged();
    }

    /// <summary>
    /// Called from JS when the OS-level color-scheme preference changes.
    /// Ignored once the user has manually toggled the theme.
    /// </summary>
    [JSInvokable]
    public void OnSystemThemeChanged(bool prefersDark)
    {
        if (_userHasToggled)
            return;

        _isDarkMode = prefersDark;
        StateHasChanged();
    }

    private void ToggleTheme()
    {
        _isDarkMode = !_isDarkMode;
        _userHasToggled = true;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("themeDetector.dispose");
        }
        catch (JSDisconnectedException)
        {
            // Runtime may already be torn down during app shutdown.
        }

        _dotNetRef?.Dispose();
    }
}
