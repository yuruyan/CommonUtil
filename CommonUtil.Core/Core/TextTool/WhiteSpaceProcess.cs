namespace CommonUtil.Core;

public static partial class WhiteSpaceProcess {
    /// <summary>
    /// 多个空白字符正则
    /// </summary>
    private static readonly Regex MultipleWhiteSpaceRegex = GetMultipleWhiteSpaceRegex();

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
    #endregion

    #region FileProcess
    /// <summary>
    /// 文件文本去除首尾空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileTrimText(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, TrimText);
    }

    /// <summary>
    /// 文件文本去除空白行
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileRemoveWhiteSpaceLine(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, RemoveWhiteSpaceLine);
    }

    /// <summary>
    /// 文件文本去除每行首尾空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileTrimLine(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, TrimLine);
    }

    /// <summary>
    /// 文件文本去除每行首部空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileTrimLineStart(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, TrimLineStart);
    }

    /// <summary>
    /// 文件文本去除每行尾部空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileTrimLineEnd(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, TrimLineEnd);
    }

    /// <summary>
    /// 文件文本将多个空白字符替换成一个空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <returns></returns>
    public static void FileReplaceMultipleWhiteSpaceWithOne(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, ReplaceMultipleWhiteSpaceWithOne);
    }
    #endregion

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"[\t\r\f ]{2,}")]
    private static partial Regex GetMultipleWhiteSpaceRegex();
#elif NET6_0_OR_GREATER
    private static Regex GetMultipleWhiteSpaceRegex() => new(@"[\t\r\f ]{2,}");
#endif
}
