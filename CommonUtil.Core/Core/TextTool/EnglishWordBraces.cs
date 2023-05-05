namespace CommonUtil.Core;

public static partial class EnglishWordProcess {
    /// <summary>
    /// 英文单词正则
    /// </summary>
    private static readonly Regex EnglishWordRegex = TextTool.EnglishWordRegex;
#if NET7_0_OR_GREATER
    /// <summary>
    /// 英文单词、数字正则
    /// </summary>
    private static readonly Regex EnglishWordNumberRegex = GetEnglishWordNumberRegex();
#elif NET6_0_OR_GREATER
    /// <summary>
    /// 英文单词、数字正则
    /// </summary>
    private static readonly Regex EnglishWordNumberRegex = new(@"\s*(?<word>[\da-z]+(?:(?:'[\da-z]+)?(?: [\da-z]+)+)?)\s*", RegexOptions.IgnoreCase);
#endif

#if NET7_0_OR_GREATER
    [GeneratedRegex("\\s*(?<word>[\\da-z]+(?:(?:'[\\da-z]+)?(?: [\\da-z]+)+)?)\\s*", RegexOptions.IgnoreCase)]
    private static partial Regex GetEnglishWordNumberRegex();
#endif

    #region TextProcess
    /// <summary>
    /// 英文两边加空格
    /// </summary>
    /// <param name="text"></param>
    /// <param name="includeNumber">包括数字</param>
    /// <returns></returns>
    public static string AddEnglishWordBraces(string text, bool includeNumber = false) {
        var regex = includeNumber ? EnglishWordNumberRegex : EnglishWordRegex;
        return regex.Replace(text, ReplaceMatchEvaluator);

        static string ReplaceMatchEvaluator(Match match) => $" {match.Groups["word"]} ";
    }

    /// <summary>
    /// 移除英文两边空格
    /// </summary>
    /// <param name="text"></param>
    /// <param name="includeNumber">包括数字</param>
    /// <returns></returns>
    public static string RemoveEnglishWordBraces(string text, bool includeNumber = false) {
        var regex = includeNumber ? EnglishWordNumberRegex : EnglishWordRegex;
        return regex.Replace(text, ReplaceMatchEvaluator);

        static string ReplaceMatchEvaluator(Match match) => $"{match.Groups["word"]}";
    }
    #endregion

    #region FileProcess

    /// <summary>
    /// 文件文本英文两边加空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="includeNumber">包括数字</param>
    /// <returns></returns>
    public static void FileAddEnglishWordBraces(string inputPath, string outputPath, bool includeNumber = false) {
        File.WriteAllText(outputPath, AddEnglishWordBraces(File.ReadAllText(inputPath), includeNumber));
    }

    /// <summary>
    /// 文件文本英文两边移除空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="includeNumber">包括数字</param>
    /// <returns></returns>
    public static void FileRemoveEnglishWordBraces(string inputPath, string outputPath, bool includeNumber = false) {
        File.WriteAllText(outputPath, RemoveEnglishWordBraces(File.ReadAllText(inputPath), includeNumber));
    }
    #endregion
}
