using System.Windows.Controls.Primitives;

namespace CommonUtil.View;

public partial class SettingsView : Page {
    public static readonly IReadOnlyDictionary<string, ThemeOptions> ThemeOptionsDict = new Dictionary<string, ThemeOptions>() {
        {"跟随系统", ThemeOptions.Auto },
        {"浅色", ThemeOptions.Light },
        {"深色", ThemeOptions.Dark },
    };

    public SettingsView() {
        InitializeComponent();
    }

    private void ThemeComboBoxSelectionChangedHandler(object sender, SelectionChangedEventArgs e) {
        if (sender is not Selector selector || selector.SelectedValue is not ThemeOptions theme) {
            return;
        }
        SystemColorsHelper.SystemThemeChanged -= SystemThemeChangedHandler;
        if (theme == ThemeOptions.Light) {
            ThemeManager.Current.SwitchToLightTheme();
        } else if (theme == ThemeOptions.Dark) {
            ThemeManager.Current.SwitchToDarkTheme();
        } else {
            SystemColorsHelper.SystemThemeChanged += SystemThemeChangedHandler;
            // Change theme
            SystemThemeChangedHandler(null, SystemColorsHelper.CurrentSystemTheme);
        }
    }

    private void SystemThemeChangedHandler(object? sender, ThemeMode e) {
        if (e == ThemeMode.Light) {
            ThemeManager.Current.SwitchToLightTheme();
        } else {
            ThemeManager.Current.SwitchToDarkTheme();
        }
    }

    /// <summary>
    /// Select the first item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ThemeComboBoxLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is Selector selector) {
            selector.Loaded -= ThemeComboBoxLoadedHandler;
            selector.SelectedIndex = 0;
        }
    }
}
