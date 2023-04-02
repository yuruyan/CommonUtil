using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System.IO.Compression;

namespace CommonUtil.Core;

[Flags]
public enum RandomStringChoice {
    None = 0,
    Number = 1,
    UpperCase = 2,
    LowerCase = 4,
    SpacialCharacter = 8
}

public static class RandomGenerator {
    private const string NumberCharacter = "0123456789";
    private const string UpperCaseCharacter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LowerCaseCharacter = "abcdefghijklmnopqrstuvwxyz";
    private const string SpacialCharacter = @"$%&'()*+,-./\:;<=>?@[]^_`{|}~";

    private const string RandomDataSourcePath = "Resources/RandomDataSource.zip";
    private const string ChineseNameEntryName = "ChineseNames.txt";
    private const string ChineseFamilyNameEntryName = "ChineseFamilyNames.txt";
    private const string JapaneseNameEntryName = "JapaneseNames.txt";
    private const string JapaneseFamilyNameEntryName = "JapaneseFamilyNames.txt";
    private const string ChineseAncientNameEntryName = "ChineseAncientNames.txt";
    private const string EnglishNameEntryName = "EnglishNames.txt";
    private const string EnglishChineseNameEntryName = "EnglishChineseNames.txt";
    private const string EnglishWordsEntryName = "EnglishWords.txt";
    private const string ChineseWordsEntryName = "ChineseWords.txt";
    /// <summary>
    /// 数据源，首次加载会有延迟
    /// </summary>
    private static readonly IReadOnlyDictionary<string, List<string>> DataSourceDict = new Dictionary<string, List<string>>() {
        {ChineseNameEntryName, new () },
        {ChineseFamilyNameEntryName, new () },
        {JapaneseNameEntryName, new () },
        {JapaneseFamilyNameEntryName, new () },
        {ChineseAncientNameEntryName, new () },
        {EnglishNameEntryName, new () },
        {EnglishChineseNameEntryName, new () },
        {EnglishWordsEntryName, new () },
        {ChineseWordsEntryName, new () },
    };

    private static readonly IRandomizerString RandomizerEmailAddress = RandomizerFactory.GetRandomizer(new FieldOptionsEmailAddress());
    private static readonly IRandomizerGuid RandomizerGuid = RandomizerFactory.GetRandomizer(new FieldOptionsGuid());
    private const int IPV4AddressFragment = 256;
    private const int IPV6AddressFragment = 65536;
    private const int MACAddressFragment = 256;

    /// <summary>
    /// 生成随机数
    /// </summary>
    /// <param name="minValue">最小值，包括</param>
    /// <param name="maxValue">最大值，不包括</param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static int[] GenerateRandomNumber(int minValue, int maxValue, uint count) {
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
    public static string[] GenerateRandomString(RandomStringChoice choice, int length, uint count) {
        return GenerateRandomString(choice, new Range(new(length), new(length)), count);
    }

    /// <summary>
    /// 生成随机字符串
    /// </summary>
    /// <param name="choice"></param>
    /// <param name="range">字符串长度范围</param>
    /// <param name="count">随机字符串个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomString(RandomStringChoice choice, Range range, uint count) {
        if (choice == RandomStringChoice.None) {
            return Array.Empty<string>();
        }
        var sb = new StringBuilder();
        if (choice.HasFlag(RandomStringChoice.Number)) {
            sb.Append(NumberCharacter);
        }
        if (choice.HasFlag(RandomStringChoice.UpperCase)) {
            sb.Append(UpperCaseCharacter);
        }
        if (choice.HasFlag(RandomStringChoice.LowerCase)) {
            sb.Append(LowerCaseCharacter);
        }
        if (choice.HasFlag(RandomStringChoice.SpacialCharacter)) {
            sb.Append(SpacialCharacter);
        }
        string dataSource = sb.ToString();
        string[] results = new string[count];
        if (string.IsNullOrEmpty(dataSource)) {
            return results;
        }
        // 随机选择字符
        for (int i = 0; i < count; i++) {
            sb.Clear();
            int length = Random.Shared.Next(range.Start.Value, range.End.Value);
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
    public static string[] GenerateRandomStringWithRegex(string regex, uint count) {
        var randomizerTextRegex = RandomizerFactory.GetRandomizer(
            new FieldOptionsTextRegex { Pattern = regex }
        );
        string[] results = new string[count];
        for (int i = 0; i < count; i++) {
            results[i] = randomizerTextRegex.Generate() ?? string.Empty;
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
    public static string[] GenerateRandomStringWithDataSource(IList<char> dataSource, Range range, uint count) {
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

    /// <summary>
    /// 根据数据源随机挑选
    /// </summary>
    /// <param name="dataSource"></param>
    /// <param name="count">生成个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomStringChoices(IList<string> dataSource, uint count) {
        string[] results = new string[count];
        if (!dataSource.Any()) {
            return results;
        }
        for (int i = 0, sourceLength = dataSource.Count; i < count; i++) {
            results[i] = dataSource[Random.Shared.Next(sourceLength)];
        }
        return results;
    }

    /// <summary>
    /// 生成随机 DateTime
    /// </summary>
    /// <param name="minDateTime"></param>
    /// <param name="maxDateTime">不包括</param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static DateTime[] GenerateRandomDateTime(DateTime minDateTime, DateTime maxDateTime, uint count) {
        long minMills = minDateTime.ToTimestamp();
        long maxMills = maxDateTime.ToTimestamp();
        long gap = maxMills - minMills;
        var data = new DateTime[count];
        for (int i = 0; i < count; i++) {
            data[i] = minDateTime.AddMilliseconds(Random.Shared.NextInt64(gap));
        }
        return data;
    }

    /// <summary>
    /// 生成随机邮箱地址
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string[] GenerateRandomEmailAddresses(uint count) {
        var data = new string[count];
        for (int i = 0; i < count; i++) {
            data[i] = RandomizerEmailAddress.Generate() ?? string.Empty;
        }
        return data;
    }

    /// <summary>
    /// 转为大写
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static string[] ToUpperCase(string[] data) {
        for (int i = 0; i < data.Length; i++) {
            data[i] = data[i].ToUpperInvariant();
        }
        return data;
    }

    /// <summary>
    /// 生成随机 guid
    /// </summary>
    /// <param name="count"></param>
    /// <param name="upperCase">是否大写</param>
    /// <returns></returns>
    public static string[] GenerateRandomGuids(uint count, bool upperCase = false) {
        var data = new string[count];
        for (int i = 0; i < count; i++) {
            data[i] = RandomizerGuid.Generate()?.ToString() ?? string.Empty;
        }
        // 大写
        if (upperCase) {
            return ToUpperCase(data);
        }
        return data;
    }

    /// <summary>
    /// 生成随机 ipv4 地址
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string[] GenerateRandomIPV4Addresses(uint count) {
        var data = new string[count];
        for (int i = 0; i < count; i++) {
            data[i] = $"{Random.Shared.Next(IPV4AddressFragment)}.{Random.Shared.Next(IPV4AddressFragment)}.{Random.Shared.Next(IPV4AddressFragment)}.{Random.Shared.Next(IPV4AddressFragment)}";
        }
        return data;
    }

    /// <summary>
    /// 生成随机 ipv6 地址
    /// </summary>
    /// <param name="count"></param>
    /// <param name="upperCase">是否大写</param>
    /// <returns></returns>
    public static string[] GenerateRandomIPV6Addresses(uint count, bool upperCase = false) {
        var data = new string[count];
        var sb = new StringBuilder(39);
        for (int i = 0; i < count; i++) {
            sb.Clear();
            for (int j = 0; j < 8; j++) {
                sb.Append($"{Convert.ToString(Random.Shared.Next(IPV6AddressFragment), 16).PadLeft(4, '0')}:");
            }
            sb.Remove(sb.Length - 1, 1);
            data[i] = sb.ToString();
        }
        // 大写
        if (upperCase) {
            return ToUpperCase(data);
        }
        return data;
    }

    /// <summary>
    /// 生成随机 MAC 地址
    /// </summary>
    /// <param name="count"></param>
    /// <param name="upperCase">是否大写</param>
    /// <returns></returns>
    public static string[] GenerateRandomMACAddresses(uint count, bool upperCase = false) {
        var data = new string[count];
        var sb = new StringBuilder(17);
        for (int i = 0; i < count; i++) {
            sb.Clear();
            for (int j = 0; j < 6; j++) {
                sb.Append($"{Convert.ToString(Random.Shared.Next(MACAddressFragment), 16).PadLeft(2, '0')}:");
            }
            sb.Remove(sb.Length - 1, 1);
            data[i] = sb.ToString();
        }
        // 大写
        if (upperCase) {
            return ToUpperCase(data);
        }
        return data;
    }

    /// <summary>
    /// 获取压缩文件内容
    /// </summary>
    /// <param name="entryName">压缩文件名</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">压缩文件找不到</exception>
    private static string GetArchiveResource(string entryName) {
        using var archiveStream = File.OpenRead(RandomDataSourcePath);
        ZipArchiveEntry? zipArchiveEntry = new ZipArchive(archiveStream).GetEntry(entryName);
        // entry 找不到
        if (zipArchiveEntry == null) {
            throw new FileNotFoundException($"压缩文件 {entryName} 找不到");
        }

        using var readStream = zipArchiveEntry.Open();
        using var readerStream = new StreamReader(readStream);
        return readerStream.ReadToEnd();
    }

    /// <summary>
    /// 生成随机数据
    /// </summary>
    /// <param name="entryName">压缩文件名称</param>
    /// <param name="count"></param>
    /// <returns></returns>
    private static string[] GenerateRandomData(string entryName, uint count) {
        var dataSource = DataSourceDict[entryName];
        // 没有加载
        if (!dataSource.Any()) {
            lock (entryName) {
                if (!dataSource.Any()) {
                    dataSource.AddRange(
                        GetArchiveResource(entryName).Split('\n')
                    );
                }
            }
        }
        var names = new string[count];
        for (int i = 0; i < count; i++) {
            names[i] = dataSource[Random.Shared.Next(dataSource.Count)];
        }
        return names;
    }

    /// <summary>
    /// 生成随机中国人名
    /// </summary>
    /// <param name="count">个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomChineseNames(uint count) => GenerateRandomData(ChineseNameEntryName, count);

    /// <summary>
    /// 生成随机中国姓氏
    /// </summary>
    /// <param name="count">生成个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomChineseFamilyNames(uint count) => GenerateRandomData(ChineseFamilyNameEntryName, count);

    /// <summary>
    /// 生成随机日本人名
    /// </summary>
    /// <param name="count">个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomJapaneseNames(uint count) => GenerateRandomData(JapaneseNameEntryName, count);

    /// <summary>
    /// 生成随机日本姓氏
    /// </summary>
    /// <param name="count">生成个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomJapaneseFamilyNames(uint count) => GenerateRandomData(JapaneseFamilyNameEntryName, count);

    /// <summary>
    /// 生成随机古代中国人名
    /// </summary>
    /// <param name="count">个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomChineseAncientNames(uint count) => GenerateRandomData(ChineseAncientNameEntryName, count);

    /// <summary>
    /// 生成随机英文名
    /// </summary>
    /// <param name="count">个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomEnglishNames(uint count) => GenerateRandomData(EnglishNameEntryName, count);

    /// <summary>
    /// 生成随机英文音译名
    /// </summary>
    /// <param name="count">个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomEnglishChineseNames(uint count) => GenerateRandomData(EnglishChineseNameEntryName, count);

    /// <summary>
    /// 生成随机英语单词
    /// </summary>
    /// <param name="count">个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomEnglishWords(uint count) => GenerateRandomData(EnglishWordsEntryName, count);

    /// <summary>
    /// 生成随机中文词语
    /// </summary>
    /// <param name="count">个数</param>
    /// <returns></returns>
    public static string[] GenerateRandomChineseWords(uint count) => GenerateRandomData(ChineseWordsEntryName, count);

}
