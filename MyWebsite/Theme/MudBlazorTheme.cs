using MudBlazor;

namespace MyWebsite.Theme;

public static class MudBlazorTheme
{
    public static MudTheme Get() =>
        new()
        {
            PaletteLight =
            {
                // Backgrounds
                Background = MudBlazorColors.Background,
                Surface = Colors.Shades.White,

                // App bar
                AppbarBackground = Colors.Shades.White,
                AppbarText = "#1A2744",

                // Drawer
                DrawerBackground = MudBlazorColors.DrawerBackground,
                DrawerText = "#E8EEF8",
                DrawerIcon = "#90AED8",

                // Brand colors
                Primary = MudBlazorColors.Primary,
                PrimaryContrastText = Colors.Shades.White,
                Secondary = MudBlazorColors.Secondary,
                SecondaryContrastText = Colors.Shades.White,
                Tertiary = MudBlazorColors.Tertiary,
                TertiaryContrastText = Colors.Shades.White,

                // Text
                TextPrimary = "#1A2744",
                TextSecondary = "#4A5568",
                TextDisabled = "#A0AEC0",

                // Lines & dividers
                Divider = "#E2E8F0",
                LinesDefault = "#E2E8F0",
                TableLines = "#E2E8F0",

                // Action
                ActionDefault = "#4A5568",
                ActionDisabled = "#A0AEC0",
                ActionDisabledBackground = "#EDF2F7"
            },
            PaletteDark =
            {
                // Backgrounds
                Background = MudBlazorColors.BackgroundDark,
                Surface = MudBlazorColors.SurfaceDark,

                // App bar
                AppbarBackground = MudBlazorColors.SurfaceDark,
                AppbarText = "#E8EEF8",

                // Drawer
                DrawerBackground = MudBlazorColors.DrawerBackgroundDark,
                DrawerText = "#C8D8F4",
                DrawerIcon = "#7BA4DC",

                // Brand colors
                Primary = MudBlazorColors.PrimaryDark,
                PrimaryContrastText = Colors.Shades.White,
                Secondary = MudBlazorColors.SecondaryDark,
                SecondaryContrastText = Colors.Shades.White,
                Tertiary = MudBlazorColors.TertiaryDark,
                TertiaryContrastText = Colors.Shades.White,

                // Text
                TextPrimary = "#E8EEF8",
                TextSecondary = "#8FA3C8",
                TextDisabled = "#4A5568",

                // Lines & dividers
                Divider = "#263354",
                LinesDefault = "#263354",
                TableLines = "#263354",

                // Action
                ActionDefault = "#8FA3C8",
                ActionDisabled = "#4A5568",
                ActionDisabledBackground = "#1E293B"
            },
            LayoutProperties = new LayoutProperties { AppbarHeight = "75px" }
        };
}
