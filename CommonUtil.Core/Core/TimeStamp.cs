namespace CommonUtil.Core;

/// <summary>
/// 时间戳 (ms)
/// </summary>
public static class TimeStamp {
    /// <summary>
    /// 获取当前时间戳
    /// </summary>
    /// <returns></returns>
    public static long GetCurrentMilliSeconds() {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 字符串时间转时间戳
    /// </summary>
    /// <param name="time"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static long StringToMilliSeconds(string time, string pattern = "yyyy-MM-dd HH:mm:ss") {
        return DateTimeUtils.ConvertToTimestamp(DateTimeUtils.ConvertToDateTime(time, pattern));
    }

    /// <summary>
    /// 时间戳转字符串时间
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string TimeStampToDateTimeString(long time) {
        return DateTimeUtils.ConvertToDateTime(time).ToString("yyyy-MM-dd HH:mm:ss");
    }
}

