using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace CommonUtil.Core;

internal class PatternParseException : ArgumentException {
    public PatternParseException(string message) : base(message) { }
}

public static partial class JsonExtractor {
    [GeneratedRegex("^(?<name>[^/\\[\\]]+)(?<arrayIdentifier>\\[(?<index>\\d+|\\*)\\])?$")]
    private static partial Regex GetPatternRegex();
    private static readonly Regex PatternRegex = GetPatternRegex();

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
    /// <exception cref="PatternParseException">解析失败</exception>
    private static IList<PatternInfo> ParsePattern(string pattern) {
        var patterns = new List<PatternInfo>();
        var paths = pattern.Split('/', StringSplitOptions.RemoveEmptyEntries);
        // 解析
        foreach (var path in paths) {
            Match match = PatternRegex.Match(path);
            // 失败
            if (!match.Success) {
                throw new PatternParseException("解析出错");
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
                // parse 失败
                if (!int.TryParse(index, out var idx)) {
                    throw new PatternParseException("解析出错");
                }
                patterns.Add(new() { Name = name, IsArray = true, Index = idx });
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
    /// <exception cref="PatternParseException">解析失败</exception>
    private static IList<string> Extract(Stream stream, string pattern) {
        var resultList = new List<string>();
        var patterns = ParsePattern(pattern);
        using var streamReader = new StreamReader(stream);
        // 解析
        var jToken = JToken.Load(new JsonTextReader(streamReader), new());
        Extract(patterns, 0, jToken, resultList);
        return resultList;
    }

    /// <summary>
    /// 数据提取
    /// </summary>
    /// <param name="patternInfos"></param>
    /// <param name="index"></param>
    /// <param name="rootToken"></param>
    /// <param name="resultList"></param>
    private static void Extract(
        IList<PatternInfo> patternInfos,
        int index,
        JToken? rootToken,
        ICollection<string> resultList
    ) {
        if (rootToken is null) {
            return;
        }
        for (int i = index; i < patternInfos.Count; i++) {
            var patternInfo = patternInfos[i];
            // 下移一层
            rootToken = rootToken[patternInfo.Name];
            // 终止
            if (rootToken is null) {
                return;
            }
            if (patternInfo.IsArray) {
                // 选择一个，继续向下
                if (!patternInfo.IsSelectAll) {
                    rootToken = rootToken.Children().Skip(patternInfo.Index).FirstOrDefault();
                    if (rootToken is null) {
                        return;
                    }
                    continue;
                }
                // 最后一层
                if (i + 2 == patternInfos.Count) {
                    var nextPatternInfo = patternInfos[i + 1];
                    foreach (var token in rootToken.Children()) {
                        resultList.Add(token[nextPatternInfo.Name]?.ToString() ?? string.Empty);
                    }
                } else {
                    foreach (var token in rootToken.Children()) {
                        Extract(patternInfos, i + 1, token, resultList);
                    }
                }
                return;
            }
        }
        resultList.Add(rootToken.ToString());
    }

    /// <summary>
    /// 数据提取
    /// </summary>
    /// <param name="text"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    /// <exception cref="PatternParseException">解析失败</exception>
    public static IList<string> Extract(string text, string pattern) {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        return Extract(stream, pattern);
    }

    /// <summary>
    /// 文件数据提取
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="pattern"></param>
    /// <exception cref="PatternParseException">解析失败</exception>
    public static void FileExtract(string inputPath, string outputPath, string pattern) {
        using var stream = File.OpenRead(inputPath);
        File.WriteAllLines(outputPath, Extract(stream, pattern));
    }
}