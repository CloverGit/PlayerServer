using Avalonia;
using Avalonia.Styling;

namespace PlayerServer.ViewModels;

public class SettingViewModel : ViewModelBase
{
    private int Theme { get; set; }

    public int ThemeSelected
    {
        get => Theme;
        set
        {
            Theme = value;
            Application.Current!.RequestedThemeVariant = Theme switch
            {
                1 => ThemeVariant.Light,
                2 => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
        }
    }
}