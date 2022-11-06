using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonUtil.Core;

public class RegexExtraction {
    private static readonly Regex GroupRegex = new(@"\\(\d+)");
    private static readonly Regex GroupSplitRegex = new(@"\\\d+");

    /// <summary>
    /// 返回与 regex 匹配的字符串列表
    /// </summary>
    /// <param name="regex"></param>
    /// <param name="input"></param>
    /// <param name="extractPattern">提取模式</param>
    /// <param name="ignoreCase">是否区分大小写</param>
    /// <returns>编译正则失败返回 null，无匹配或 extractPattern 无效返回空列表</returns>
    public static IList<string>? Extract(string regex, string input, string extractPattern = "\\0", bool ignoreCase = true) {
        Regex re;
        // 编译正则
        try {
            re = new Regex(
                regex,
                (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None) | RegexOptions.ECMAScript
            );
        } catch {
            return null;
        }
        // 普通字符串
        string[] normalStrings = GroupSplitRegex.Split(extractPattern);
        // 分组下标
        var groupIndexes = GetExtractPatternGroupIndexes(extractPattern);
        // 检查 ExtractPattern
        if (!CheckExtractPattern(groupIndexes, re.Match(input))) {
            return new List<string>();
        }
        var resultList = new List<string>();
        var sb = new StringBuilder();
        foreach (var match in re.Matches(input).Cast<Match>()) {
            resultList.Add(JoinGroupMatch(
                match,
                groupIndexes,
                normalStrings,
                sb
            ));
        }
        return resultList;
    }

    /// <summary>
    /// 文件正则提取
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="regex"></param>
    /// <param name="extractPattern">提取模式</param>
    /// <param name="ignoreCase">是否区分大小写</param>
    public static void FileExtract(
        string inputPath,
        string outputPath,
        string regex,
        string extractPattern = "\\0",
        bool ignoreCase = true
    ) {
        File.WriteAllText(
            outputPath,
            string.Join('\n', Extract(
                regex,
                File.ReadAllText(inputPath),
                extractPattern,
                ignoreCase
            ) ?? Enumerable.Empty<string>())
        );
    }

    private static string JoinGroupMatch(
        Match match,
        IList<int> groupIndexes,
        IList<string> normalStrings,
        StringBuilder sharedStringBuilder
    ) {
        var sb = sharedStringBuilder;
        sb.Clear();
        // 分组匹配结果
        var matchValues = groupIndexes
            .Select(index => match.Groups[index].Value)
            .ToArray();
        // 拼接
        for (int i = 0; i < groupIndexes.Count; i++) {
            sb.Append(normalStrings[i]).Append(matchValues[i]);
        }
        sb.Append(normalStrings[^1]);
        return sb.ToString();
    }

    /// <summary>
    /// 检查 ExtractPattern 合法性
    /// </summary>
    /// <param name="indexes"></param>
    /// <param name="firstMatch">输入第一个匹配</param>
    /// <returns>firstMatch 没有匹配返回 true</returns>
    private static bool CheckExtractPattern(IEnumerable<int> indexes, Match firstMatch) {
        if (!firstMatch.Success) {
            return true;
        }
        int groupCount = firstMatch.Groups.Count;
        return indexes.All(index => index < groupCount);
    }

    /// <summary>
    /// 获取 ExtractPattern 分组下标
    /// </summary>
    /// <param name="extractPattern"></param>
    /// <returns></returns>
    private static IList<int> GetExtractPatternGroupIndexes(string extractPattern) {
        var indexes = new List<int>();
        foreach (var match in GroupRegex.Matches(extractPattern).Cast<Match>()) {
            indexes.Add(int.Parse(match.Groups[1].Value));
        }
        return indexes;
    }
}
