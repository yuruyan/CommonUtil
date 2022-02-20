using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CommonUtil.Core;

public class RegexExtraction {
    /// <summary>
    /// 返回与 regex 匹配的字符串列表
    /// </summary>
    /// <param name="regex"></param>
    /// <param name="input"></param>
    /// <param name="ignoreCase"></param>
    /// <returns>编译正则失败返回 null，无匹配返回空列表</returns>
    public static List<string>? Extract(string regex, string input, bool? ignoreCase = true) {
        Regex re;
        try {
            re = new Regex(regex, (ignoreCase == true ? RegexOptions.IgnoreCase : RegexOptions.None) | RegexOptions.ECMAScript);
        } catch {
            return null;
        }
        var resultList = new List<string>();
        foreach (Match match in re.Matches(input)) {
            resultList.Add(match.Value);
        }
        return resultList;
    }
}

