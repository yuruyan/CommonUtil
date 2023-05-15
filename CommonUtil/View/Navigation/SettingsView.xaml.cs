using System.Windows.Controls.Primitives;

namespace CommonUtil.View;

public partial class SettingsView : Page {
    public static readonly IReadOnlyDictionary<string, ThemeOptions> ThemeOptionsDict = new Dictionary<string, ThemeOptions>() {
        {"跟随系统", ThemeOptions.Auto },
        {"浅色", ThemeOptions.Light },
        {"深色", ThemeOptions.Dark },
    };
    public static readonly DependencyProperty IsWindowTopMostProperty = DependencyProperty.Register("IsWindowTopMost", typeof(bool), typeof(SettingsView), new PropertyMetadata(false, IsWindowTopMostPropertyChangedHandler));
    private const string SystemFontSizeKey = "SystemFontSize";

    /// <summary>
    /// 窗口是否置顶
    /// </summary>
    public bool IsWindowTopMost {
        get { return (bool)GetValue(IsWindowTopMostProperty); }
        set { SetValue(IsWindowTopMostProperty, value); }
    }

    public SettingsView() {
        InitializeComponent();
        Loaded += (_, _) => FontSizeComboBox.SelectedItem = Convert.ToInt32(Application.Current.Resources[SystemFontSizeKey]);
    }

    private void ThemeComboBoxSelectionChangedHandler(object sender, SelectionChangedEventArgs e) {
        if (sender is not Selector selector || selector.SelectedValue is not ThemeOptions theme) {
            return;
        }

        if (theme == ThemeOptions.Light) {
            ThemeManager.Current.SwitchToLightTheme();
        } else if (theme == ThemeOptions.Dark) {
            ThemeManager.Current.SwitchToDarkTheme();
        } else {
            ThemeManager.Current.SwitchToAutoTheme();
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

    /// <summary>
    /// Update window TopMost property
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void IsWindowTopMostPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (Window.GetWindow(d) is Window window) {
            window.Topmost = (bool)e.NewValue;
        }
    }

    private void FontSizeComboBoxSelectionChangedHandler(object sender, SelectionChangedEventArgs e) {
        if (sender is ComboBox box && box.SelectedItem is int fontsize) {
            Application.Current.Resources[SystemFontSizeKey] = fontsize;
        }
    }
}
