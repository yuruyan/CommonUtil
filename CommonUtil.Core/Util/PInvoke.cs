using System.Runtime.InteropServices;

namespace CommonUtil.Core.Util;

internal static partial class PInvoke {
    /// <summary>
    /// Retrieves or sets the value of one of the system-wide parameters. This function can also update the user profile while setting a parameter.
    /// </summary>
    /// <param name="uAction"></param>
    /// <param name="uParam"></param>
    /// <param name="lpvParam"></param>
    /// <param name="init"></param>
    /// <returns></returns>
    /// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-systemparametersinfoa"/>
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern bool SystemParametersInfo(uint uAction, uint uParam, StringBuilder lpvParam, uint init);
}
