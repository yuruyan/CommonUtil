using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtil.Core;

[Flags]
public enum RandomStringChoice {
    None = 0,
    Number = 1,
    UpperCase = 2,
    LowerCase = 4,
    SpacialCharacter = 8
}

public class RandomGenerator {
    private static readonly string NumberCharacter = "0123456789";
    private static readonly string UpperCaseCharacter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly string LowerCaseCharacter = "abcdefghijklmnopqrstuvwxyz";
    private static readonly string SpacialCharacter = @"$%&'()*+,-./\:;<=>?@[]^_`{|}~";

    /// <summary>
    /// 生成随机数
    /// </summary>
    /// <param name="minValue">最小值，包括</param>
    /// <param name="maxValue">最大值，不包括</param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static int[] GenerateRandomNumber(int minValue, int maxValue, int count) {
        int[] results = new int[count];
        for (int i = 0; i < count; i++) {
            results[i] = Random.Shared.Next(minValue, maxValue);
        }
        return results;
    }

    /// <summary>
    /// 生成随机字符串
    /// </summary>
    /// <param name="choice"></param>
    /// <param name="length">字符串长度</param>
    /// <param name="count">随机字符串个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomString(RandomStringChoice choice, int length, int count) {
        if (choice == RandomStringChoice.None) {
            return Array.Empty<string>();
        }
        string dataSource = "";
        if (choice.HasFlag(RandomStringChoice.Number)) {
            dataSource += NumberCharacter;
        }
        if (choice.HasFlag(RandomStringChoice.UpperCase)) {
            dataSource += UpperCaseCharacter;
        }
        if (choice.HasFlag(RandomStringChoice.LowerCase)) {
            dataSource += LowerCaseCharacter;
        }
        if (choice.HasFlag(RandomStringChoice.SpacialCharacter)) {
            dataSource += SpacialCharacter;
        }
        string[] results = new string[count];
        if (string.IsNullOrEmpty(dataSource)) {
            return results;
        }
        // 随机选择字符
        var sb = new StringBuilder(length);
        for (int i = 0; i < count; i++) {
            sb.Clear();
            for (int j = 0; j < length; j++) {
                sb.Append(dataSource[Random.Shared.Next(dataSource.Length)]);
            }
            results[i] = sb.ToString();
        }
        return results;
    }

    /// <summary>
    /// 根据正则生成随机字符串
    /// </summary>
    /// <param name="regex">生成正则表达式</param>
    /// <param name="count">生成个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomStringWithRegex(string regex, int count) {
        var randomizerTextRegex = RandomizerFactory.GetRandomizer(
            new FieldOptionsTextRegex { Pattern = regex }
        );
        string[] results = new string[count];
        for (int i = 0; i < count; i++) {
            results[i] = randomizerTextRegex.Generate();
        }
        return results;
    }

    /// <summary>
    /// 根据数据源生成随机字符串
    /// </summary>
    /// <param name="dataSource"></param>
    /// <param name="range">生成字符串长度</param>
    /// <param name="count">生成个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomStringWithDataSource(IList<char> dataSource, Range range, int count) {
        string[] results = new string[count];
        if (!dataSource.Any()) {
            return results;
        }
        // 随机选择字符
        var sb = new StringBuilder(range.End.Value);
        for (int i = 0; i < count; i++) {
            sb.Clear();
            int length = Random.Shared.Next(range.Start.Value, range.End.Value);
            for (int j = 0; j < length; j++) {
                sb.Append(dataSource[Random.Shared.Next(dataSource.Count)]);
            }
            results[i] = sb.ToString();
        }
        return results;
    }
}
