namespace MyWebsite.Layout;

public partial class MainLayout
{
    private bool _isDarkMode = true;

    private void ToggleTheme() => _isDarkMode = !_isDarkMode;
}
