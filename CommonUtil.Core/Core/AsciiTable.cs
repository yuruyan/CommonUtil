using CommonUtil.Core.Model;
using Newtonsoft.Json;

namespace CommonUtil.Core;

public static class AsciiTable {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 全部字符
    /// </summary>
    private static readonly List<AsciiInfo> AsciiInfoList = new();
    /// <summary>
    /// 控制字符
    /// </summary>
    private static readonly List<AsciiInfo> AsciiInfoControlList = new();
    /// <summary>
    /// 非控制字符
    /// </summary>
    private static readonly List<AsciiInfo> AsciiInfoNormalList = new();
    /// <summary>
    /// 扩展字符
    /// </summary>
    private static readonly List<AsciiInfo> AsciiInfoExtendedList = new();

    /// <summary>
    /// 加载数据
    /// </summary>
    static AsciiTable() {
        var list = JsonConvert.DeserializeObject<List<AsciiInfo>>(
            Encoding.UTF8.GetString(Resource.Resource.Ascii)
        );
        if (list == null) {
            throw new Exception("加载 AsciiTable 失败");
        }
        AsciiInfoList.AddRange(list);
        #region 控制字符
        for (int i = 0; i < 32; i++) {
            AsciiInfoControlList.Add(AsciiInfoList[i]);
        }
        AsciiInfoControlList.Add(AsciiInfoList[127]);
        #endregion
        #region 非控制字符
        for (int i = 32; i < 127; i++) {
            AsciiInfoNormalList.Add(AsciiInfoList[i]);
        }
        #endregion
        #region 其他字符
        for (int i = 128; i < 256; i++) {
            AsciiInfoNormalList.Add(AsciiInfoList[i]);
        }
        #endregion
    }

    /// <summary>
    /// 返回全部 ASCII 列表
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyList<AsciiInfo> GetAsciiInfoList() {
        return AsciiInfoList;
    }

    /// <summary>
    /// 返回控制 ASCII 列表
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyList<AsciiInfo> GetAsciiInfoControlList() {
        return AsciiInfoControlList;
    }

    /// <summary>
    /// 返回非控制 ASCII 列表
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyList<AsciiInfo> GetAsciiInfoNormalList() {
        return AsciiInfoNormalList;
    }

    /// <summary>
    /// 返回扩展 ASCII 列表
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyList<AsciiInfo> GetAsciiInfoExtendedList() {
        return AsciiInfoExtendedList;
    }
}

