using Csv;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace CommonUtil.Core;

#if NET7_0_OR_GREATER
public static partial class JsonExtractor {
#elif NET6_0_OR_GREATER
public static class JsonExtractor {
#endif
#if NET7_0_OR_GREATER
    [GeneratedRegex("^(?<name>[^/\\[\\]]+)(?<arrayIdentifier>\\[(?<index>\\d+|\\*)\\])?$")]
    private static partial Regex GetPatternRegex();
    [GeneratedRegex("\\[(?<index>\\d+|\\*)\\]")]
    private static partial Regex GetIndexRegex();

    private static readonly Regex PatternRegex = GetPatternRegex();
    private static readonly Regex IndexRegex = GetIndexRegex();
#elif NET6_0_OR_GREATER
    private static readonly Regex PatternRegex = new(@"^(?<name>[^/\[\]]+)(?<arrayIdentifier>\[(?<index>\d+|\*)\])?$");
    private static readonly Regex IndexRegex = new(@"\[(?<index>\d+|\*)\]");
#endif
    private struct PatternInfo {
        // 根节点为数组时可为空
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
        int currentIndex = 0;
        // 解析
        foreach (var path in paths) {
            // 第一个，判断是否是数组，没有前缀
            if (currentIndex++ == 0 && (path.StartsWith('[') && path.EndsWith(']'))) {
                Match firstMatch = IndexRegex.Match(path);
                // 失败
                if (!firstMatch.Success) {
                    throw new PatternParseException("解析出错");
                }
                patterns.Add(ParseIndex(firstMatch.Groups["index"].Value));
                continue;
            }
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
            PatternInfo patternInfo = ParseIndex(match.Groups["index"].Value);
            patternInfo.Name = name;
            patterns.Add(patternInfo);
        }
        return patterns;
    }

    /// <summary>
    /// 解析 Index
    /// </summary>
    /// <param name="indexVal"></param>
    /// <returns></returns>
    /// <exception cref="PatternParseException"></exception>
    private static PatternInfo ParseIndex(string indexVal) {
        // 全部
        if (indexVal == "*") {
            return new() { IsArray = true, IsSelectAll = true };
        } else {
            // parse 失败
            if (!int.TryParse(indexVal, out var idx)) {
                throw new PatternParseException("解析出错");
            }
            return new() { IsArray = true, Index = idx };
        }
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
        // 不用 using
        var streamReader = new StreamReader(stream);
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
            // 根节点且为数组时不用下移
            if (!(index == 0 && string.IsNullOrEmpty(patternInfo.Name))) {
                // 下移一层
                rootToken = rootToken[patternInfo.Name];
            }
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
    /// <param name="json"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    /// <inheritdoc cref="Extract(Stream, string)"/>
    public static IList<string> Extract(string json, string pattern) {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        return Extract(stream, pattern);
    }

    /// <summary>
    /// 提取多列数据
    /// </summary>
    /// <param name="json"></param>
    /// <param name="patterns"></param>
    /// <returns>每列具有相同的长度</returns>
    /// <inheritdoc cref="Extract(string, string)"/>
    public static IList<IList<string>> Extract(string json, IEnumerable<string> patterns) {
        var data = new List<IList<string>>();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        foreach (var pattern in patterns) {
            stream.Seek(0, SeekOrigin.Begin);
            data.Add(Extract(stream, pattern));
        }
        var itemLength = data.Max(list => list.Count);
        // 填充值
        data.ForEach(list => list.ResizeToLength(itemLength, string.Empty));
        return data;
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

    /// <summary>
    /// 文件提取多列，文件保存为 CSV 格式
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="patterns"></param>
    public static void FileExtract(string inputPath, string outputPath, IEnumerable<string> patterns) {
        using var readStream = File.OpenRead(inputPath);
        var data = new List<IList<string>>();
        foreach (var pattern in patterns) {
            readStream.Seek(0, SeekOrigin.Begin);
            data.Add(Extract(readStream, pattern));
        }
        var itemLength = data.Max(list => list.Count);
        // 填充值
        data.ForEach(list => list.ResizeToLength(itemLength, string.Empty));

        #region Construct Record
        var headers = GetPatternHeaders(patterns).ToArray();
        var records = new List<string[]>();
        for (int i = 0; i < itemLength; i++) {
            var record = new string[headers.Length];
            for (int j = 0; j < record.Length; j++) {
                record[j] = data[j][i];
            }
            records.Add(record);
        }
        #endregion

        #region Write to CSV
        using var streamWriter = new StreamWriter(File.OpenWrite(outputPath));
        CsvWriter.Write(streamWriter, headers, records);
        #endregion
    }

    /// <summary>
    /// 获取 Pattern Headers
    /// </summary>
    /// <param name="patterns"></param>
    /// <returns></returns>
    public static IList<string> GetPatternHeaders(IEnumerable<string> patterns) {
        var headers = new List<string>();
        var newPatters = patterns
            .Select(p => p.Trim())
            .Select(p => p.Trim('/'))
            .Where(p => !string.IsNullOrEmpty(p))
            .ToArray();
        for (int i = 0; i < newPatters.Length; i++) {
            var pattern = newPatters[i];
            var indexHeader = $"Column {i + 1}";
            var lastSplashIndex = pattern.LastIndexOf('/');
            if (lastSplashIndex != -1) {
                var trimedHeader = pattern[lastSplashIndex..].Trim().Trim('/');
                headers.Add(string.IsNullOrEmpty(trimedHeader) ? indexHeader : trimedHeader);
            } else {
                headers.Add(indexHeader);
            }
        }
        return headers;
    }
}