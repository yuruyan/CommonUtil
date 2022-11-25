using CommonUITools.Utils;
using CommonUtil.Model;
using NLog;
using System;
using System.Linq;
using System.Windows;

namespace CommonUtil.Store;

internal class ThemeManager : DependencyObject {
    private const string LightThemeSource = "/CommonUtil;component/Resource/ResourceDictionary/LightThemeResources.xaml";
    private const string DarkThemeSource = "/CommonUtil;component/Resource/ResourceDictionary/DarkThemeResources.xaml";
    private const string CommonUIToolsLightThemeSource = "/CommonUITools;component/Resource/ResourceDictionary/LightThemeResources.xaml";
    private const string CommonUIToolsDarkThemeSource = "/CommonUITools;component/Resource/ResourceDictionary/DarkThemeResources.xaml";

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
        ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Light;
        ReplaceResourceDictionary(CommonUIToolsDarkThemeSource, CommonUIToolsLightThemeSource);
        ReplaceResourceDictionary(DarkThemeSource, LightThemeSource);
        ThemeChanged?.Invoke(Current, ThemeMode.Light);
    }

    /// <summary>
    /// 替换 ResourceDictionary
    /// </summary>
    /// <param name="oldSource"></param>
    /// <param name="newSource"></param>
    /// <returns></returns>
    private bool ReplaceResourceDictionary(string oldSource, string newSource) {
        var resources = App.Current.Resources.MergedDictionaries;
        var oldResource = resources.FirstOrDefault(r => r.Source != null && r.Source.OriginalString == oldSource);
        // 找不到
        if (oldResource == null) {
            Logger.Error($"Cannot find the resource {oldSource}");
            return false;
        }
        // 替换
        resources[resources.IndexOf(oldResource)] = new() {
            Source = new(newSource, UriKind.Relative)
        };
        return true;
    }

    /// <summary>
    /// 切换为 LightTheme
    /// </summary>
    public void SwitchToDarkTheme() {
        if (CurrentMode == ThemeMode.Dark) {
            return;
        }
        CurrentMode = ThemeMode.Dark;
        ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;
        ReplaceResourceDictionary(CommonUIToolsLightThemeSource, CommonUIToolsDarkThemeSource);
        ReplaceResourceDictionary(LightThemeSource, DarkThemeSource);
        ThemeChanged?.Invoke(Current, ThemeMode.Dark);
    }
}
