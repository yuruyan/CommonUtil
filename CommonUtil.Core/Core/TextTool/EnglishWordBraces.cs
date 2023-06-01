namespace CommonUtil.Core;

public static partial class EnglishWordProcess {
    private const string EnglishWordNumberPattern = @"[\t ]*(?<word>[\da-z]+(?:(?:'[\da-z]+)?(?: [\da-z]+)+)?)[\t ]*";
    private const string ASCIIWordPattern = @"[\t ]*(?<word>[\x21-\x7e]+)[\t ]*";

    /// <summary>
    /// 英文单词正则
    /// </summary>
    private static readonly Regex EnglishPhraseRegex = TextTool.EnglishPhraseRegex;
    /// <summary>
    /// 英文单词、数字正则
    /// </summary>
    private static readonly Regex EnglishWordNumberRegex = GetEnglishWordNumberRegex();
    /// <summary>
    /// ASCII word
    /// </summary>
    private static readonly Regex ASCIIWordRegex = GetASCIIWordRegex();

#if NET7_0_OR_GREATER
    [GeneratedRegex(EnglishWordNumberPattern, RegexOptions.IgnoreCase)]
    private static partial Regex GetEnglishWordNumberRegex();
    [GeneratedRegex(ASCIIWordPattern, RegexOptions.IgnoreCase)]
    private static partial Regex GetASCIIWordRegex();
#else
    private static Regex GetEnglishWordNumberRegex() => new(EnglishWordNumberPattern, RegexOptions.IgnoreCase);
    private static Regex GetASCIIWordRegex() => new(ASCIIWordPattern);
#endif

    #region TextProcess
    /// <summary>
    /// 英文两边加空格
    /// </summary>
    /// <param name="text"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static string AddEnglishWordBraces(string text, EnglishWordBracesMode mode) {
        var regex = mode switch {
            EnglishWordBracesMode.IncludeNumber => EnglishWordNumberRegex,
            EnglishWordBracesMode.IncludeASCII => ASCIIWordRegex,
            _ => EnglishPhraseRegex
        };
        return regex.Replace(text, ReplaceMatchEvaluator);

        static string ReplaceMatchEvaluator(Match match) => $" {match.Groups["word"]} ";
    }

    /// <summary>
    /// 移除英文两边空格
    /// </summary>
    /// <param name="text"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static string RemoveEnglishWordBraces(string text, EnglishWordBracesMode mode) {
        var regex = mode switch {
            EnglishWordBracesMode.IncludeNumber => EnglishWordNumberRegex,
            EnglishWordBracesMode.IncludeASCII => ASCIIWordRegex,
            _ => EnglishPhraseRegex
        };
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
    /// <param name="mode"></param>
    /// <returns></returns>
    public static void FileAddEnglishWordBraces(string inputPath, string outputPath, EnglishWordBracesMode mode) {
        File.WriteAllText(outputPath, AddEnglishWordBraces(File.ReadAllText(inputPath), mode));
    }

    /// <summary>
    /// 文件文本英文两边移除空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static void FileRemoveEnglishWordBraces(string inputPath, string outputPath, EnglishWordBracesMode mode) {
        File.WriteAllText(outputPath, RemoveEnglishWordBraces(File.ReadAllText(inputPath), mode));
    }
    #endregion
}
