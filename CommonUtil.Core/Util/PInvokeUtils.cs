namespace CommonUtil.Core.Util;

public static class PInvokeUtils {
    const uint SPI_GETDESKWALLPAPER = 0x0073;

    /// <summary>
    /// 获取桌面壁纸路径
    /// </summary>
    /// <returns>失败返回 null</returns>
    public static string? GetDesktopWallpaper() {
        var wallPaperPath = new StringBuilder(byte.MaxValue);
        return PInvoke.SystemParametersInfo(
            SPI_GETDESKWALLPAPER,
            byte.MaxValue,
            wallPaperPath,
            0
        ) ? wallPaperPath.ToString() : null;
    }
}
