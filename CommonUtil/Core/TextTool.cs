using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommonUtil.Core;

public class TextTool {
    /// <summary>
    /// 多个空白字符正则
    /// </summary>
    private static readonly Regex MultipleWhiteSpace = new (@"[\t\r\f ]{2,}");

    /// <summary>
    /// 文本去重
    /// </summary>
    /// <param name="text"></param>
    /// <param name="splitSymbol">分隔符</param>
    /// <param name="mergeSymbol">合并符</param>
    /// <param name="trim">移除元素首尾空白</param>
    /// <returns></returns>
    public static string RemoveDuplicate(string text, string splitSymbol, string mergeSymbol, bool trim = false) {
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
}
