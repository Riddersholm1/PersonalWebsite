using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyWebsite.Layout;

public partial class MainLayout : IAsyncDisposable
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    private bool _isDarkMode = true;
    private bool _userHasToggled;
    private bool _drawerOpen;
    private string _activeSection = "home";
    private DotNetObjectReference<MainLayout>? _dotNetRef;

    private static readonly NavSection[] _sections =
    [
        new("home", "Hjem", Icons.Material.Filled.Home),
        new("about", "Om mig", Icons.Material.Filled.Person),
        new("skills", "Kompetencer", Icons.Material.Filled.Code),
        new("projects", "Projekter", Icons.Material.Filled.Folder),
        new("experience", "Erfaring", Icons.Material.Filled.Work),
        new("education", "Uddannelse", Icons.Material.Filled.School),
        new("contact", "Kontakt", Icons.Material.Filled.Email),
    ];

    private string ActiveSectionLabel =>
        _sections.FirstOrDefault(s => s.Id == _activeSection)?.Label ?? "Hjem";

    private bool IsActive(string sectionId) => _activeSection == sectionId;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        _dotNetRef = DotNetObjectReference.Create(this);

        var prefersDark = await JS.InvokeAsync<bool>("themeDetector.init", _dotNetRef);
        _isDarkMode = prefersDark;

        await JS.InvokeVoidAsync("sectionObserver.init", _dotNetRef);

        StateHasChanged();
    }

    [JSInvokable]
    public void SetActiveSection(string sectionId)
    {
        if (_activeSection == sectionId)
            return;

        _activeSection = sectionId;
        StateHasChanged();
    }

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

    private void ToggleDrawer() => _drawerOpen = !_drawerOpen;

    private void CloseDrawer() => _drawerOpen = false;

    public async ValueTask DisposeAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("sectionObserver.dispose");
            await JS.InvokeVoidAsync("themeDetector.dispose");
        }
        catch (JSDisconnectedException)
        {
            // Runtime may already be torn down during app shutdown.
        }

        _dotNetRef?.Dispose();
    }

    private sealed record NavSection(string Id, string Label, string Icon);
}
