using System;
using System.Collections.Generic;
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
            return string.Join(mergeSymbol, new HashSet<string>(text.Split(splitSymbol)));
        }
        return string.Join(mergeSymbol, new HashSet<string>(text.Split(splitSymbol, StringSplitOptions.TrimEntries)));
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
    /// 去除空白行
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveWhiteSpaceLine(string text) {
        return string.Join(Environment.NewLine, text.Split(Environment.NewLine).Where(s => s.Trim().Any()));
    }

    /// <summary>
    /// 去除每行首尾空格
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string TrimLine(string text) {
        return string.Join(Environment.NewLine, text.Split(Environment.NewLine).Select(s => s.Trim()));
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
}
