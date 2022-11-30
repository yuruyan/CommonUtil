using CommonUITools.Model;
using CommonUITools.Utils;
using NLog;
using System;
using System.Windows;

namespace CommonUtil.Store;

internal class ThemeManager : DependencyObject {
    private const string LightThemeSource = "/CommonUtil;component/Resource/ResourceDictionary/LightThemeResources.xaml";
    private const string DarkThemeSource = "/CommonUtil;component/Resource/ResourceDictionary/DarkThemeResources.xaml";

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public event EventHandler<ThemeMode>? ThemeChanged;
    public static readonly ThemeManager Current;
    public ThemeMode CurrentMode { get; private set; } = ThemeMode.Light;

    static ThemeManager() {
        Current = UIUtils.RunOnUIThread(() => new ThemeManager());
    }

    private ThemeManager() { }

    /// <summary>
    /// 切换为 LightTheme
    /// </summary>
    public void SwitchToLightTheme() {
        if (CurrentMode == ThemeMode.Light) {
            return;
        }
        CurrentMode = ThemeMode.Light;
        ReplaceResourceDictionary(DarkThemeSource, LightThemeSource);
        CommonUITools.Themes.ThemeManager.SwitchToLightTheme();
        ThemeChanged?.Invoke(Current, ThemeMode.Light);
    }

    /// <summary>
    /// 切换为 DarkTheme
    /// </summary>
    public void SwitchToDarkTheme() {
        if (CurrentMode == ThemeMode.Dark) {
            return;
        }
        CurrentMode = ThemeMode.Dark;
        ReplaceResourceDictionary(LightThemeSource, DarkThemeSource);
        CommonUITools.Themes.ThemeManager.SwitchToDarkTheme();
        ThemeChanged?.Invoke(Current, ThemeMode.Dark);
    }

    /// <summary>
    /// 替换 ResourceDictionary
    /// </summary>
    /// <param name="oldSource"></param>
    /// <param name="newSource"></param>
    /// <returns></returns>
    private bool ReplaceResourceDictionary(string oldSource, string newSource) {
        return UIUtils.ReplaceResourceDictionary(
            App.Current.Resources.MergedDictionaries,
            oldSource,
            newSource
        );
    }
}
