using Newtonsoft.Json;

namespace CommonUtil.Core;

/// <summary>
/// 简繁体转换
/// </summary>
public static class ChineseTransform {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// (繁体，简体) Dict
    /// </summary>
    private static readonly IDictionary<char, char> TraditionalSimplifiedMap;
    /// <summary>
    /// (简体，繁体) Dict
    /// </summary>
    private static readonly IDictionary<char, char> SimplifiedTraditionalMap;

    /// <summary>
    /// 加载数据
    /// </summary>
    static ChineseTransform() {
        TraditionalSimplifiedMap = JsonConvert.DeserializeObject<Dictionary<char, char>>(
           Encoding.UTF8.GetString(Resource.Resource.ChineseCharacterMap)
        )!;
        SimplifiedTraditionalMap = new Dictionary<char, char>();
        // 填充 SimplifiedTraditionalMap
        foreach (var item in TraditionalSimplifiedMap) {
            SimplifiedTraditionalMap[item.Value] = item.Key;
        }
        Logger.Debug("加载中文简繁体完毕");
    }

    /// <summary>
    /// 显式初始化，默认隐式初始化
    /// </summary>
    public static void InitializeExplicitly() => _ = TraditionalSimplifiedMap;

    /// <summary>
    /// 转繁体字
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToTraditional(string s) => new(ToTraditional(s.ToCharArray()));

    /// <summary>
    /// 转繁体
    /// </summary>
    /// <param name="text"></param>
    /// <returns>同 <paramref name="text"/></returns>
    public static char[] ToTraditional(char[] text) => ToTraditional(text, 0, text.Length);

    /// <summary>
    /// 转繁体，转换从 <paramref name="startIndex"/> 开始的 <paramref name="count"/> 个字符
    /// </summary>
    /// <param name="text"></param>
    /// <param name="startIndex">开始索引</param>
    /// <param name="count">个数</param>
    /// <returns>同 <paramref name="text"/></returns>
    public static char[] ToTraditional(char[] text, int startIndex, int count) {
        for (int i = startIndex; i < startIndex + count; i++) {
            var ch = text[i];
            text[i] = SimplifiedTraditionalMap.TryGetValue(ch, out var value) ? value : ch;
        }
        return text;
    }

    /// <summary>
    /// 转简体
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToSimplified(string s) => new(ToSimplified(s.ToCharArray()));

    /// <summary>
    /// 转简体
    /// </summary>
    /// <param name="text"></param>
    /// <returns>同 <paramref name="text"/></returns>
    public static char[] ToSimplified(char[] text) => ToSimplified(text, 0, text.Length);

    /// <summary>
    /// 转简体，转换从 <paramref name="startIndex"/> 开始的 <paramref name="count"/> 个字符
    /// </summary>
    /// <param name="text"></param>
    /// <param name="startIndex">开始索引</param>
    /// <param name="count">个数</param>
    /// <returns>同 <paramref name="text"/></returns>
    public static char[] ToSimplified(char[] text, int startIndex, int count) {
        for (int i = startIndex; i < startIndex + count; i++) {
            var ch = text[i];
            text[i] = TraditionalSimplifiedMap.TryGetValue(ch, out var value) ? value : ch;
        }
        return text;
    }

    /// <summary>
    /// 文件转繁体
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="token"></param>
    /// <param name="processCallback">进度回调，参数为进度百分比</param>
    public static void FileToTraditional(string inputPath, string outputPath, CancellationToken? token = null, Action<double>? processCallback = null) {
        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);
        var buffer = new char[ConstantUtils.DefaultFileBufferSize];
        long totalLength = reader.BaseStream.Length, totalReadCount = 0;
        int readCount;
        while ((readCount = reader.Read(buffer, 0, buffer.Length)) > 0) {
            // 中断
            if (token?.IsCancellationRequested == true) {
                return;
            }
            totalReadCount += readCount;
            processCallback?.Invoke((double)totalReadCount / totalLength);
            writer.Write(
                ToTraditional(buffer, 0, readCount),
                0,
                readCount
            );
        }
        processCallback?.Invoke(1);
    }

    /// <summary>
    /// 文件转简体
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="token"></param>
    /// <param name="processCallback">进度回调，参数为进度百分比</param>
    public static void FileToSimplified(string inputPath, string outputPath, CancellationToken? token = null, Action<double>? processCallback = null) {
        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);
        var buffer = new char[ConstantUtils.DefaultFileBufferSize];
        long totalLength = reader.BaseStream.Length, totalReadCount = 0;
        int readCount;
        while ((readCount = reader.Read(buffer, 0, buffer.Length)) > 0) {
            // 中断
            if (token?.IsCancellationRequested == true) {
                return;
            }
            totalReadCount += readCount;
            processCallback?.Invoke((double)totalReadCount / totalLength);
            writer.Write(
                ToSimplified(buffer, 0, readCount),
                0,
                readCount
            );
        }
        processCallback?.Invoke(1);
    }
}
