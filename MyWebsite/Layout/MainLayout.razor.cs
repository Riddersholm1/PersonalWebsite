using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyWebsite.Layout;

public partial class MainLayout : IAsyncDisposable
{
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private bool _isDarkMode = true;
    private bool _drawerOpen;
    private string _activeSection = "home";
    private DotNetObjectReference<MainLayout>? _dotNetRef;

    private bool IsActiveSection(string sectionId) =>
        string.Equals(_activeSection, sectionId, StringComparison.OrdinalIgnoreCase);

    private void ToggleTheme() => _isDarkMode = !_isDarkMode;

    private void ToggleDrawer() => _drawerOpen = !_drawerOpen;

    private void CloseDrawer() => _drawerOpen = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("sectionObserver.init", _dotNetRef);
        }
    }

    [JSInvokable]
    public void SetActiveSection(string sectionId)
    {
        _activeSection = sectionId;
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        if (_dotNetRef is not null)
        {
            await JS.InvokeVoidAsync("sectionObserver.dispose");
            _dotNetRef.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
