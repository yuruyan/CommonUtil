using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace CommonUtil.Core;

public static class JsonExtractor {
    private static readonly Regex PatternRegex = new(@"^(?<name>[^/\[\]]+)(?<arrayIdentifier>\[(?<index>\d+|\*)\])?$");

    private struct PatternInfo {
        public string Name { get; set; }
        public bool IsArray { get; set; }
        public bool IsSelectAll { get; set; }
        public int Index { get; set; }

        public override string ToString() {
            return $"{{{nameof(Name)}={Name}, {nameof(IsArray)}={IsArray.ToString()}, {nameof(IsSelectAll)}={IsSelectAll.ToString()}, {nameof(Index)}={Index.ToString()}}}";
        }
    }

    /// <summary>
    /// 解析 pattern
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">解析失败</exception>
    private static IEnumerable<PatternInfo> ParsePattern(string pattern) {
        var patterns = new List<PatternInfo>();
        var paths = pattern.Split('/', StringSplitOptions.RemoveEmptyEntries);
        // 解析
        foreach (var path in paths) {
            Match match = PatternRegex.Match(path);
            // 失败
            if (!match.Success) {
                throw new ArgumentException("解析出错");
            }
            string name = match.Groups["name"].Value;
            // 不是数组
            if (!match.Groups["arrayIdentifier"].Success) {
                patterns.Add(new() { Name = name });
                continue;
            }
            string index = match.Groups["index"].Value;
            // 全部
            if (index == "*") {
                patterns.Add(new() { Name = name, IsArray = true, IsSelectAll = true });
            } else {
                patterns.Add(new() { Name = name, IsArray = true, Index = int.Parse(index) });
            }
        }
        return patterns;
    }

    /// <summary>
    /// 数据提取
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static IEnumerable<string> Extract(Stream stream, string pattern) {
        var resultList = new List<string>();
        var patterns = ParsePattern(pattern);
        using var streamReader = new StreamReader(stream);
        // 解析
        var jToken = JToken.Load(new JsonTextReader(streamReader));
        // 没有内容
        if (jToken is null) {
            return resultList;
        }
        var enumerator = patterns.GetEnumerator();
        while (enumerator.MoveNext()) {
            var patternInfo = enumerator.Current;
            // 下移一层
            jToken = jToken[patternInfo.Name];
            // 没有内容
            if (jToken is null) {
                return resultList;
            }
            if (patternInfo.IsArray) {
                if (enumerator.MoveNext()) {
                    resultList.AddRange(ExtractArray(jToken, patternInfo, enumerator.Current.Name));
                }
                return resultList;
            }
        }
        return resultList;
    }

    /// <summary>
    /// 数据提取
    /// </summary>
    /// <param name="text"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static IEnumerable<string> Extract(string text, string pattern) {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        return Extract(stream, pattern);
    }

    /// <summary>
    /// 文件数据提取
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="pattern"></param>
    public static void FileExtract(string inputPath, string outputPath, string pattern) {
        using var stream = File.OpenRead(inputPath);
        File.WriteAllLines(outputPath, Extract(stream, pattern));
    }

    /// <summary>
    /// 提取集合
    /// </summary>
    /// <param name="jToken"></param>
    /// <param name="arrayPatternInfo"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    private static IEnumerable<string> ExtractArray(JToken jToken, PatternInfo arrayPatternInfo, string memberName) {
        string ExtractSelector(JToken t) => t[memberName]?.ToString() ?? string.Empty;
        var children = jToken.Children();
        // 提取全部
        if (arrayPatternInfo.IsSelectAll) {
            return children.Select(ExtractSelector);
        }
        return children.Skip(arrayPatternInfo.Index).Take(1).Select(ExtractSelector);
    }
}