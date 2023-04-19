using System.Text.RegularExpressions;

namespace CommonUtil.Core;

public static partial class TextTool {
    public delegate string TextProcess(string text);
    public delegate void FileProcess(string inputFile, string outputFile);

    #region Field
    /// <summary>
    /// 英文句子分隔符
    /// </summary>
    private const char EnglishSentenceSeparator = '.';
#if NET7_0_OR_GREATER
    /// <summary>
    /// 英文单词正则
    /// </summary>
    private static readonly Regex EnglishWordRegex = GetEnglishWordRegex();
    /// <summary>
    /// 英文单词、数字正则
    /// </summary>
    private static readonly Regex EnglishWordNumberRegex = GetEnglishWordNumberRegex();
    /// <summary>
    /// 多个空白字符正则
    /// </summary>
    private static readonly Regex MultipleWhiteSpaceRegex = GetMultipleWhiteSpaceRegex();
#elif NET6_0_OR_GREATER
    /// <summary>
    /// 英文单词正则
    /// </summary>
    private static readonly Regex EnglishWordRegex = new(@"\s*(?<word>[a-z]+(?:(?:'[a-z]+)?(?: [a-z]+)+)?)\s*", RegexOptions.IgnoreCase);
    /// <summary>
    /// 英文单词、数字正则
    /// </summary>
    private static readonly Regex EnglishWordNumberRegex = new(@"\s*(?<word>[\da-z]+(?:(?:'[\da-z]+)?(?: [\da-z]+)+)?)\s*", RegexOptions.IgnoreCase);
    /// <summary>
    /// 多个空白字符正则
    /// </summary>
    private static readonly Regex MultipleWhiteSpaceRegex = new(@"[\t\r\f ]{2,}");
#endif
    /// <summary>
    /// 半角全角 Dict
    /// </summary>
    private static readonly IReadOnlyDictionary<char, char> HalfFullCharDict;
    /// <summary>
    /// 全角半角 Dict
    /// </summary>
    private static readonly IReadOnlyDictionary<char, char> FullHalfCharDict;
    #endregion

#if NET7_0_OR_GREATER
    [GeneratedRegex("\\s*(?<word>[a-z]+(?:(?:'[a-z]+)?(?: [a-z]+)+)?)\\s*", RegexOptions.IgnoreCase)]
    private static partial Regex GetEnglishWordRegex();
    [GeneratedRegex("\\s*(?<word>[\\da-z]+(?:(?:'[\\da-z]+)?(?: [\\da-z]+)+)?)\\s*", RegexOptions.IgnoreCase)]
    private static partial Regex GetEnglishWordNumberRegex();
    [GeneratedRegex("[\\t\\r\\f ]{2,}")]
    private static partial Regex GetMultipleWhiteSpaceRegex();
#endif

    static TextTool() {
        var halfFullCharDict = new Dictionary<char, char>();
        var fullHalfCharDict = new Dictionary<char, char>();
        // 空格
        halfFullCharDict[(char)32u] = (char)12288;
        // 其余字符
        for (char i = (char)33; i < 127; i++) {
            halfFullCharDict[i] = (char)(i + 65248);
        }
        // 填充 FullHalfCharDict
        foreach (var item in halfFullCharDict) {
            fullHalfCharDict[item.Value] = item.Key;
        }
        HalfFullCharDict = halfFullCharDict;
        FullHalfCharDict = fullHalfCharDict;
    }

    #region 文本处理
    /// <summary>
    /// 文本去重
    /// </summary>
    /// <param name="text"></param>
    /// <param name="splitSymbol">分隔符</param>
    /// <param name="mergeSymbol">合并符</param>
    /// <param name="trim">移除元素首尾空白</param>
    /// <returns></returns>
    public static string RemoveDuplicate(string text, string splitSymbol, string mergeSymbol, bool trim = false) {
        //分隔每个字符
        if (splitSymbol == string.Empty) {
            return string.Join(mergeSymbol, text.ToHashSet());
        }
        if (!trim) {
            return string.Join(
                mergeSymbol,
                new HashSet<string>(text.ReplaceLineFeedWithLinuxStyle().Split(splitSymbol))
            );
        }
        return string.Join(
            mergeSymbol,
            new HashSet<string>(
                text.ReplaceLineFeedWithLinuxStyle().Split(splitSymbol, StringSplitOptions.TrimEntries)
            )
        );
    }

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
    /// 半角转全角
    /// </summary>
    /// <param name="halfCharText"></param>
    /// <returns></returns>
    public static string HalfCharToFullChar(string halfCharText) {
        char[] array = halfCharText.ToCharArray();
        for (int i = 0; i < array.Length; i++) {
            if (HalfFullCharDict.ContainsKey(array[i])) {
                array[i] = HalfFullCharDict[array[i]];
            }
        }
        return new(array);
    }

    /// <summary>
    /// 全角转半角
    /// </summary>
    /// <param name="fullCharText"></param>
    /// <returns></returns>
    public static string FullCharToHalfChar(string fullCharText) {
        char[] array = fullCharText.ToCharArray();
        for (int i = 0; i < array.Length; i++) {
            if (FullHalfCharDict.ContainsKey(array[i])) {
                array[i] = FullHalfCharDict[array[i]];
            }
        }
        return new(array);
    }

    /// <summary>
    /// 添加行号
    /// </summary>
    /// <param name="text"></param>
    /// <param name="separator">数字与文本分隔符</param>
    /// <returns></returns>
    public static string PrependLineNumber(string text, string separator) {
        string[] lines = text.ReplaceLineFeedWithLinuxStyle().Split('\n');
        for (int i = 0; i < lines.Length; i++) {
            lines[i] = $"{i + 1}{separator}" + lines[i];
        }
        return string.Join('\n', lines);
    }

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

    /// <summary>
    /// 将多个空白字符替换成一个空格
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ReplaceMultipleWhiteSpaceWithOne(string text) {
        return MultipleWhiteSpaceRegex.Replace(text, " ");
    }

    /// <summary>
    /// 小写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToLowerCase(string text) => text.ToLowerInvariant();

    /// <summary>
    /// 大写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToUpperCase(string text) => text.ToUpperInvariant();

    /// <summary>
    /// 切换大小写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToggleCase(string text) {
        char[] chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++) {
            char ch = chars[i];
            chars[i] = ch switch {
                >= 'a' and <= 'z' => char.ToUpper(ch),
                >= 'A' and <= 'Z' => char.ToLower(ch),
                _ => ch
            };
        }
        return new(chars);
    }

    /// <summary>
    /// 将每个单词首字母大写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string CapitalizeWords(string text)
        => EnglishWordRegex.Replace(
            ToLowerCase(text),
            match => CapitalizeFirstWordCharacter(match.Value)
        );

    /// <summary>
    /// 将文本第一个字母大写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static string CapitalizeFirstWordCharacter(string text) {
        var chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++) {
            char ch = chars[i];
            if (char.IsLetter(ch)) {
                chars[i] = char.ToUpper(ch);
                break;
            }
        }
        return new(chars);
    }

    /// <summary>
    /// 将每一句话首字母大写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToSentenceCase(string text) {
        var lines = ToLowerCase(text).Split('\n');
        // 对每一行根据 EnglishSentenceSeparator 分割
        for (int i = 0; i < lines.Length; i++) {
            var sentences = lines[i].Split(EnglishSentenceSeparator);
            for (int j = 0; j < sentences.Length; j++) {
                sentences[j] = CapitalizeFirstWordCharacter(sentences[j]);
            }
            lines[i] = string.Join(EnglishSentenceSeparator, sentences);
        }
        return string.Join('\n', lines);
    }

    /// <summary>
    /// 翻转文本
    /// </summary>
    /// <param name="text"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static string InvertText(string text, InversionMode mode) {
        text = text.ReplaceLineFeedWithLinuxStyle();
        return mode switch {
            InversionMode.Horizontal => string.Join('\n', text.Split('\n').Select(s => string.Join("", s.Reverse()))),
            InversionMode.Vertical => string.Join('\n', text.Split('\n').Reverse()),
            InversionMode.Both => string.Join("", text.Reverse()),
            _ => InvertText(text, InversionMode.Both),
        };
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
    private static void ProcessFileText(string inputPath, string outputPath, Func<string, string> func) {
        File.WriteAllText(outputPath, func(File.ReadAllText(inputPath)));
    }

    /// <summary>
    /// 文件文本去重
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="splitSymbol">分隔符</param>
    /// <param name="mergeSymbol">合并符</param>
    /// <param name="trim">移除元素首尾空白</param>
    public static void FileRemoveDuplicate(
        string inputPath,
        string outputPath,
        string splitSymbol,
        string mergeSymbol,
        bool trim = false
    ) {
        File.WriteAllText(
            outputPath,
            RemoveDuplicate(
                File.ReadAllText(inputPath),
                splitSymbol,
                mergeSymbol,
                trim
            )
        );
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
    /// 文件文本半角转全角
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileHalfCharToFullChar(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, HalfCharToFullChar);
    }

    /// <summary>
    /// 文件文本全角转半角
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileFullCharToHalfChar(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, FullCharToHalfChar);
    }

    /// <summary>
    /// 文件文本添加行号
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="separator">数字与文本分隔符</param>
    /// <returns></returns>
    public static void FilePrependLineNumber(string inputPath, string outputPath, string separator) {
        File.WriteAllText(outputPath, PrependLineNumber(File.ReadAllText(inputPath), separator));
    }

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

    /// <summary>
    /// 文件文本小写
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileToLowerCase(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, ToLowerCase);
    }

    /// <summary>
    /// 文件文本大写
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileToUpperCase(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, ToUpperCase);
    }

    /// <summary>
    /// 文件文本切换大小写
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileToggleCase(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, ToggleCase);
    }

    /// <summary>
    /// 文件文本将每个单词首字母大写
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileCapitalizeWords(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, CapitalizeWords);
    }

    /// <summary>
    /// 文件文本将每一句话首字母大写
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileToSentenceCase(string inputPath, string outputPath) {
        ProcessFileText(inputPath, outputPath, ToSentenceCase);
    }

    /// <summary>
    /// 文件文本翻转
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="mode"></param>
    public static void FileInvertText(string inputPath, string outputPath, InversionMode mode) {
        File.WriteAllText(outputPath, InvertText(File.ReadAllText(inputPath), mode));
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
