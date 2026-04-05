using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyWebsite.Layout;

public partial class MainLayout : IAsyncDisposable
{
    private static readonly string[] SectionIds = ["home", "about", "skills", "experience", "projects", "education", "contact"];

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    private IJSObjectReference? _module;
    private DotNetObjectReference<MainLayout>? _dotNetReference;
    private bool _isDarkMode = true;
    private string _activeSection = "home";

    protected override void OnInitialized()
    {
        _isDarkMode = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/scrollSpy.js");
        _dotNetReference = DotNetObjectReference.Create(this);
        await _module.InvokeVoidAsync("initializeScrollSpy", _dotNetReference, SectionIds);
    }

    [JSInvokable]
    public Task SetActiveSection(string section)
    {
        if (string.Equals(_activeSection, section, StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        _activeSection = section;
        return InvokeAsync(StateHasChanged);
    }

    private bool IsActiveSection(string section)
        => string.Equals(_activeSection, section, StringComparison.OrdinalIgnoreCase);

    private void ToggleTheme()
    {
        _isDarkMode = !_isDarkMode;
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.InvokeVoidAsync("disposeScrollSpy");
            await _module.DisposeAsync();
        }

        _dotNetReference?.Dispose();
    }
}