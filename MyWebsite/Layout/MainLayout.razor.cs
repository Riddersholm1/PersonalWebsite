using MudBlazor;

namespace MyWebsite.Layout;

public partial class MainLayout
{
    private MudThemeProvider _mudThemeProvider = null!;
    private bool _isDarkMode;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _isDarkMode = await _mudThemeProvider.GetSystemDarkModeAsync();
        await _mudThemeProvider.WatchSystemDarkModeAsync(OnSystemDarkModeChanged);
        StateHasChanged();
    }

    private Task OnSystemDarkModeChanged(bool newValue)
    {
        _isDarkMode = newValue;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private void ToggleTheme()
    {
        _isDarkMode = !_isDarkMode;
    }
}