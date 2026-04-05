using MudBlazor;

namespace MyWebsite.Theme;

public static class MudBlazorTheme
{
    public static MudTheme Get() =>
        new()
        {
            PaletteLight =
            {
                AppbarBackground = Colors.Shades.White,
                AppbarText = Colors.Shades.Black,
                DrawerBackground = MudBlazorColors.Primary,
                Primary = MudBlazorColors.Primary,
                Secondary = MudBlazorColors.Secondary,
                Tertiary = Colors.Shades.White,
                DrawerText = Colors.Shades.White,
                DrawerIcon = Colors.Shades.White,
                TextPrimary = Colors.Shades.Black,
                Background = MudBlazorColors.Background
            },
            LayoutProperties = new LayoutProperties { AppbarHeight = "75px" }
        };
}
