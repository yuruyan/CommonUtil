using CommonUITools.Utils;
using CommonUtil.Model;
using System;
using System.Windows;

namespace CommonUtil.Store;

internal class ThemeManager : DependencyObject {
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
        ThemeChanged?.Invoke(Current, ThemeMode.Light);
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
        ThemeChanged?.Invoke(Current, ThemeMode.Dark);
    }
}
