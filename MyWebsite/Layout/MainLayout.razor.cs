using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyWebsite.Layout;

public partial class MainLayout : IAsyncDisposable
{
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private bool _isDarkMode = true;
    private string _activeSection = "home";
    private DotNetObjectReference<MainLayout>? _dotNetRef;

    private bool IsActiveSection(string sectionId) =>
        string.Equals(_activeSection, sectionId, StringComparison.OrdinalIgnoreCase);

    private void ToggleTheme() => _isDarkMode = !_isDarkMode;

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
