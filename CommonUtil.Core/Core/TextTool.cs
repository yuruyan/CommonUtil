namespace CommonUtil.Core;

public static partial class TextTool {
    public delegate string TextProcess(string text);
    public delegate void FileProcess(string inputFile, string outputFile);

    #region Field
    /// <summary>
    /// 英文句子分隔符
    /// </summary>
    internal const char EnglishSentenceSeparator = '.';
#if NET7_0_OR_GREATER
    /// <summary>
    /// 英文单词正则
    /// </summary>
    internal static readonly Regex EnglishWordRegex = GetEnglishWordRegex();
    /// <summary>
    /// 多个空白字符正则
    /// </summary>
    internal static readonly Regex MultipleWhiteSpaceRegex = GetMultipleWhiteSpaceRegex();
#elif NET6_0_OR_GREATER
    /// <summary>
    /// 英文单词正则
    /// </summary>
    internal static readonly Regex EnglishWordRegex = new(@"\s*(?<word>[a-z]+(?:(?:'[a-z]+)?(?: [a-z]+)+)?)\s*", RegexOptions.IgnoreCase);
    /// <summary>
    /// 多个空白字符正则
    /// </summary>
    internal static readonly Regex MultipleWhiteSpaceRegex = new(@"[\t\r\f ]{2,}");
#endif
    #endregion

#if NET7_0_OR_GREATER
    [GeneratedRegex("\\s*(?<word>[a-z]+(?:(?:'[a-z]+)?(?: [a-z]+)+)?)\\s*", RegexOptions.IgnoreCase)]
    private static partial Regex GetEnglishWordRegex();
    [GeneratedRegex("[\\t\\r\\f ]{2,}")]
    private static partial Regex GetMultipleWhiteSpaceRegex();
#endif

    #region 文本处理
    /// <summary>
    /// 去除空白行
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveWhiteSpaceLine(string text) {
        return string.Join(
            '\n',
            text.ReplaceLineFeedWithLinuxStyle()
                .Split('\n')
                .Where(s => s.Trim().Any())
        );
    }

    /// <summary>
    /// 去除首尾空格
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string TrimText(string text) {
        return text.Trim();
    }

    /// <summary>
    /// 去除每行首尾空格
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string TrimLine(string text) {
        return string.Join(
            '\n',
            text.ReplaceLineFeedWithLinuxStyle()
                .Split('\n')
                .Select(s => s.Trim())
        );
    }

    /// <summary>
    /// 去除每行首部空格
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string TrimLineStart(string text) {
        return string.Join(
            '\n',
            text.ReplaceLineFeedWithLinuxStyle()
                .Split('\n')
                .Select(s => s.TrimStart())
        );
    }

    /// <summary>
    /// 去除每行尾部空格
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string TrimLineEnd(string text) {
        return string.Join(
            '\n',
            text.ReplaceLineFeedWithLinuxStyle()
                .Split('\n')
                .Select(s => s.TrimEnd())
        );
    }

    /// <summary>
    /// 将多个空白字符替换成一个空格
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ReplaceMultipleWhiteSpaceWithOne(string text) {
        return MultipleWhiteSpaceRegex.Replace(text, " ");
    }

    /// <summary>
    /// 排序文本行
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string SortLines(string text) {
        var lines = text.Split('\n');
        Array.Sort(lines);
        return string.Join('\n', lines);
    }

    /// <summary>
    /// 打乱文本行
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    public static IList<string> ShuffleLines(IEnumerable<string> lines) {
        var data = lines.ToList();
        for (int i = 0, total = data.Count; i < total; i++) {
            var index = Random.Shared.Next(total - i);
            var replaceIndex = total - i - 1;
            (data[index], data[replaceIndex]) = (data[replaceIndex], data[index]);
        }
        return data;
    }
    #endregion

    #region 文件处理
    /// <summary>
    /// 文件文本处理
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="func"></param>
    internal static void ProcessFileText(string inputPath, string outputPath, Func<string, string> func) {
        File.WriteAllText(outputPath, func(File.ReadAllText(inputPath)));
    }

    /// <summary>
    /// 文件文本去除首尾空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileTrimText(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, TrimText);
    }

    /// <summary>
    /// 文件文本去除空白行
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileRemoveWhiteSpaceLine(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, RemoveWhiteSpaceLine);
    }

    /// <summary>
    /// 文件文本去除每行首尾空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileTrimLine(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, TrimLine);
    }

    /// <summary>
    /// 文件文本去除每行首部空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileTrimLineStart(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, TrimLineStart);
    }

    /// <summary>
    /// 文件文本去除每行尾部空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileTrimLineEnd(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, TrimLineEnd);
    }

    /// <summary>
    /// 文件文本将多个空白字符替换成一个空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <returns></returns>
    public static void FileReplaceMultipleWhiteSpaceWithOne(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, ReplaceMultipleWhiteSpaceWithOne);
    }

    /// <summary>
    /// 文件排序文本行
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileSortLines(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, SortLines);
    }

    /// <summary>
    /// 文件打乱文本行
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileShuffleLines(string inputPath, string outputPath) {
        File.WriteAllLines(outputPath, ShuffleLines(File.ReadAllLines(inputPath)));
    }
    #endregion
}
