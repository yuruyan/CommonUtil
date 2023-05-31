using System.Windows.Interop;
using PInvoke;

namespace CommonUtil.Utils;

internal static class SystemMenuUtils {
    /// <summary>
    /// WM_SYSCOMMAND <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/wm-syscommand"/>
    /// </summary>
    private const int WM_SYSCOMMAND = 0x0112;
    private const int SettingId = 0x10;
    public static event EventHandler? SettingClicked;

    /// <summary>
    /// RegisterMenus
    /// </summary>
    /// <param name="window"></param>
    /// <remarks>
    /// Note window must be SourceInitialized
    /// </remarks>
    public static void RegisterMenus(Window window) {
        var hs = (HwndSource)PresentationSource.FromVisual(window);
        hs.AddHook(new HwndSourceHook(WindowMessageProcess));
        IntPtr hSysMenu = User32.GetSystemMenu(hs.Handle, false);
        // Add a separator
        User32.AppendMenu(hSysMenu, User32.MenuItemFlags.MF_SEPARATOR, 0, string.Empty);
        User32.AppendMenu(hSysMenu, User32.MenuItemFlags.MF_STRING, SettingId, "Setting");
    }

    private static nint WindowMessageProcess(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) {
        if (msg == WM_SYSCOMMAND && wParam == SettingId) {
            SettingClicked?.Invoke(null, null!);
        }
        return IntPtr.Zero;
    }
}
