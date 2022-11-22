﻿using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommonUtil.Core;

/// <summary>
/// 简繁体转换
/// </summary>
public static class ChineseTransform {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// (繁体，简体) Dict
    /// </summary>
    private static readonly IDictionary<char, char> TraditionalSimplifiedMap;
    /// <summary>
    /// (简体，繁体) Dict
    /// </summary>
    private static readonly IDictionary<char, char> SimplifiedTraditionalMap;

    /// <summary>
    /// 加载数据
    /// </summary>
    static ChineseTransform() {
        TraditionalSimplifiedMap = JsonConvert.DeserializeObject<Dictionary<char, char>>(
           Encoding.UTF8.GetString(Resource.Resource.ChineseCharacterMap)
        )!;
        SimplifiedTraditionalMap = new Dictionary<char, char>();
        // 填充 SimplifiedTraditionalMap
        foreach (var item in TraditionalSimplifiedMap) {
            SimplifiedTraditionalMap[item.Value] = item.Key;
        }
        Logger.Debug("加载中文简繁体完毕");
    }

    /// <summary>
    /// 显式初始化，默认隐式初始化
    /// </summary>
    public static void InitializeExplicitly() => _ = TraditionalSimplifiedMap;

    /// <summary>
    /// 转繁体字
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToTraditional(string s) {
        var sb = new StringBuilder();
        foreach (var item in s) {
            sb.Append(SimplifiedTraditionalMap.TryGetValue(item, out var value) ? value : item);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 转简体
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToSimplified(string s) {
        var sb = new StringBuilder();
        foreach (var item in s) {
            sb.Append(TraditionalSimplifiedMap.TryGetValue(item, out var value) ? value : item);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 文件转繁体
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileToTraditional(string inputPath, string outputPath) {
        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);
        string? data = null;
        while ((data = reader.ReadLine()) != null) {
            writer.WriteLine(ToTraditional(data));
        }
    }

    /// <summary>
    /// 文件转简体
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileToSimplified(string inputPath, string outputPath) {
        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);
        string? data = null;
        while ((data = reader.ReadLine()) != null) {
            writer.WriteLine(ToSimplified(data));
        }
    }
}
