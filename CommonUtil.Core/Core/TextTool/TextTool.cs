namespace CommonUtil.Core;

public static partial class TextTool {
    public delegate string TextProcess(string text);
    public delegate void FileProcess(string inputFile, string outputFile);

    #region Field
    /// <summary>
    /// 英文句子分隔符
    /// </summary>
    internal const char EnglishSentenceSeparator = '.';
    /// <summary>
    /// 英文单词正则
    /// </summary>
    internal static readonly Regex EnglishPhraseRegex = GetEnglishPhraseRegex();
    #endregion

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"\s*(?<word>[a-z]+(?:(?:'[a-z]+)?(?: [a-z]+)+)?)\s*", RegexOptions.IgnoreCase)]
    private static partial Regex GetEnglishPhraseRegex();
#elif NET6_0_OR_GREATER
    private static Regex GetEnglishPhraseRegex() => new(@"\s*(?<word>[a-z]+(?:(?:'[a-z]+)?(?: [a-z]+)+)?)\s*", RegexOptions.IgnoreCase);
#endif

    /// <summary>
    /// 文件文本处理
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="func"></param>
    internal static void ProcessFileText(string inputPath, string outputPath, Func<string, string> func) {
        File.WriteAllText(outputPath, func(File.ReadAllText(inputPath)));
    }
}
