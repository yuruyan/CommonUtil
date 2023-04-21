namespace CommonUtil.Theme;

internal class ThemeManager : DependencyObject {
    private const string LightThemeSource = "/CommonUtil;component/Resources/ResourceDictionary/LightThemeResources.xaml";
    private const string DarkThemeSource = "/CommonUtil;component/Resources/ResourceDictionary/DarkThemeResources.xaml";

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ObservableProperty<ThemeMode> ThemeModeProperty = ThemeMode.Light;

    public event EventHandler<ThemeMode>? ThemeChanged;
    public static readonly ThemeManager Current;
    public ThemeMode CurrentMode => ThemeModeProperty.Value;

    static ThemeManager() {
        Current = UIUtils.RunOnUIThread(() => new ThemeManager());
    }

    private ThemeManager() {
        ThemeModeProperty.ValueChanged += ThemeModeChanged;
    }

    private void ThemeModeChanged(ThemeMode oldVal, ThemeMode newVal) {
        if (newVal == ThemeMode.Light) {
            ReplaceResourceDictionary(DarkThemeSource, LightThemeSource);
            CommonUITools.Themes.ThemeManager.SwitchToLightTheme();
            ThemeChanged?.Invoke(Current, ThemeMode.Light);
        } else {
            ReplaceResourceDictionary(LightThemeSource, DarkThemeSource);
            CommonUITools.Themes.ThemeManager.SwitchToDarkTheme();
            ThemeChanged?.Invoke(Current, ThemeMode.Dark);
        }
    }

    /// <summary>
    /// 切换为 LightTheme
    /// </summary>
    public void SwitchToLightTheme() {
        SystemColorsHelper.SystemThemeChanged -= SystemThemeChangedHandler;
        ThemeModeProperty.Value = ThemeMode.Light;
    }

    /// <summary>
    /// 切换为 DarkTheme
    /// </summary>
    public void SwitchToDarkTheme() {
        SystemColorsHelper.SystemThemeChanged -= SystemThemeChangedHandler;
        ThemeModeProperty.Value = ThemeMode.Dark;
    }

    /// <summary>
    /// 跟随系统
    /// </summary>
    public void SwitchToAutoTheme() {
        SystemColorsHelper.SystemThemeChanged += SystemThemeChangedHandler;
        // Change theme
        SystemThemeChangedHandler(null, SystemColorsHelper.CurrentSystemTheme);
    }

    private void SystemThemeChangedHandler(object? sender, ThemeMode e) {
        ThemeModeProperty.Value = e == ThemeMode.Light
            ? ThemeMode.Light
            : ThemeMode.Dark;
    }

    /// <summary>
    /// 替换 ResourceDictionary
    /// </summary>
    /// <param name="oldSource"></param>
    /// <param name="newSource"></param>
    /// <returns></returns>
    private bool ReplaceResourceDictionary(string oldSource, string newSource) {
        return App.Current.Resources.MergedDictionaries.ReplaceResourceDictionary(
            oldSource,
            newSource
        );
    }
}
