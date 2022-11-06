using CommonUITools.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommonUtil.Core;

public class TextTool {
    /// <summary>
    /// 半角全角 Dict
    /// </summary>
    private static readonly Dictionary<char, char> HalfFullCharDict = new();
    /// <summary>
    /// 全角半角 Dict
    /// </summary>
    private static readonly Dictionary<char, char> FullHalfCharDict = new();

    /// <summary>
    /// 多个空白字符正则
    /// </summary>
    private static readonly Regex MultipleWhiteSpace = new(@"[\t\r\f ]{2,}");

    static TextTool() {
        // 空格
        HalfFullCharDict[(char)32] = (char)12288;
        // 其余字符
        for (char i = (char)33; i < 127; i++) {
            HalfFullCharDict[i] = (char)(i + 65248);
        }
        // 填充 FullHalfCharDict
        foreach (var item in HalfFullCharDict) {
            FullHalfCharDict[item.Value] = item.Key;
        }
    }

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
                new HashSet<string>(CommonUtils.NormalizeMultipleLineText(text).Split(splitSymbol))
            );
        }
        return string.Join(
            mergeSymbol,
            new HashSet<string>(
                CommonUtils.NormalizeMultipleLineText(text).Split(splitSymbol, StringSplitOptions.TrimEntries)
            )
        );
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
    /// 去除首尾空格
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string TrimText(string text) {
        return text.Trim();
    }

    /// <summary>
    /// 文件文本去除首尾空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileTrimText(string inputPath, string outputPath) {
        File.WriteAllText(outputPath, TrimText(File.ReadAllText(inputPath)));
    }

    /// <summary>
    /// 去除空白行
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveWhiteSpaceLine(string text) {
        return string.Join(
            '\n',
            CommonUtils.NormalizeMultipleLineText(text)
                .Split('\n')
                .Where(s => s.Trim().Any())
        );
    }

    /// <summary>
    /// 文件文本去除空白行
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileRemoveWhiteSpaceLine(string inputPath, string outputPath) {
        File.WriteAllText(outputPath, RemoveWhiteSpaceLine(File.ReadAllText(inputPath)));
    }

    /// <summary>
    /// 去除每行首尾空格
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string TrimLine(string text) {
        return string.Join(
            '\n',
            CommonUtils.NormalizeMultipleLineText(text)
                .Split('\n')
                .Select(s => s.Trim())
        );
    }

    /// <summary>
    /// 文件文本去除每行首尾空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileTrimLine(string inputPath, string outputPath) {
        File.WriteAllText(outputPath, TrimLine(File.ReadAllText(inputPath)));
    }

    /// <summary>
    /// 将多个空白字符替换成一个空格
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ReplaceMultipleWhiteSpaceWithOne(string text) {
        return MultipleWhiteSpace.Replace(text, " ");
    }

    /// <summary>
    /// 文件文本将多个空白字符替换成一个空格
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <returns></returns>
    public static void FileReplaceMultipleWhiteSpaceWithOne(string inputPath, string outputPath) {
        File.WriteAllText(outputPath, ReplaceMultipleWhiteSpaceWithOne(File.ReadAllText(inputPath)));
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
    /// 文件文本半角转全角
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileHalfCharToFullChar(string inputPath, string outputPath) {
        File.WriteAllText(outputPath, HalfCharToFullChar(File.ReadAllText(inputPath)));
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
    /// 文件文本全角转半角
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileFullCharToHalfChar(string inputPath, string outputPath) {
        File.WriteAllText(outputPath, FullCharToHalfChar(File.ReadAllText(inputPath)));
    }

    /// <summary>
    /// 添加行号
    /// </summary>
    /// <param name="text"></param>
    /// <param name="separator">数字与文本分隔符</param>
    /// <returns></returns>
    public static string PrependLineNumber(string text, string separator) {
        string[] lines = CommonUtils.NormalizeMultipleLineText(text).Split('\n');
        for (int i = 0; i < lines.Length; i++) {
            lines[i] = $"{i + 1}{separator}" + lines[i];
        }
        return string.Join('\n', lines);
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
    /// 英文单词正则
    /// </summary>
    public static readonly Regex EnglishWordRegex = new(@"[a-z]+", RegexOptions.IgnoreCase);

    /// <summary>
    /// 英文单词、数字正则
    /// </summary>
    public static readonly Regex EnglishWordNumberRegex = new(@"[a-z0-9]+", RegexOptions.IgnoreCase);

    /// <summary>
    /// 英文两边加空格
    /// </summary>
    /// <param name="text"></param>
    /// <param name="includeNumber">包括数字</param>
    /// <returns></returns>
    public static string AddEnglishWordBraces(string text, bool includeNumber = false) {
        if (includeNumber) {
            return EnglishWordNumberRegex.Replace(text, match => $" {match} ");
        }
        return EnglishWordRegex.Replace(text, match => $" {match} ");
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
}
