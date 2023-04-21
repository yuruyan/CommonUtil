namespace CommonUtil.Core;

public static class AsciiTable {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 全部字符
    /// </summary>
    public static IReadOnlyList<AsciiInfo> AsciiInfoList { get; }
    /// <summary>
    /// 控制字符
    /// </summary>
    public static IReadOnlyList<AsciiInfo> AsciiInfoControlList { get; }
    /// <summary>
    /// 非控制字符
    /// </summary>
    public static IReadOnlyList<AsciiInfo> AsciiInfoNormalList { get; }
    /// <summary>
    /// 扩展字符
    /// </summary>
    public static IReadOnlyList<AsciiInfo> AsciiInfoExtendedList { get; }

    /// <summary>
    /// 加载数据
    /// </summary>
    static AsciiTable() {
        var asciiInfoList = new List<AsciiInfo>(DataResourceHelper.AsciiInfoList);
        var asciiInfoControlList = new List<AsciiInfo>();
        var asciiInfoNormalList = new List<AsciiInfo>();
        var asciiInfoExtendedList = new List<AsciiInfo>();

        #region 控制字符
        for (int i = 0; i < 32; i++) {
            asciiInfoControlList.Add(asciiInfoList[i]);
        }
        asciiInfoControlList.Add(asciiInfoList[127]);
        #endregion

        #region 非控制字符
        for (int i = 32; i < 127; i++) {
            asciiInfoNormalList.Add(asciiInfoList[i]);
        }
        #endregion

        #region 其他字符
        for (int i = 128; i < 256; i++) {
            asciiInfoNormalList.Add(asciiInfoList[i]);
        }
        #endregion

        AsciiInfoList = asciiInfoList;
        AsciiInfoControlList = asciiInfoControlList;
        AsciiInfoNormalList = asciiInfoNormalList;
        AsciiInfoExtendedList = asciiInfoExtendedList;
    }
}
