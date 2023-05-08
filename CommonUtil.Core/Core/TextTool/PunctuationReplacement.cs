namespace CommonUtil.Core;

public static class PunctuationReplacement {
    /// <summary>
    /// 英文中文标点符号
    /// </summary>
    private static readonly IReadOnlyDictionary<char, char> EnglishChinesePunctuationDict = new Dictionary<char, char>() {
        {'!', '！'},
        {'(', '（'},
        {')', '）'},
        {'\'', '\''},
        {'"', '"'},
        {';', '；'},
        {':', '：'},
        {',', '，'},
        {'.', '。'},
        {'?', '？'},
    };
    /// <summary>
    /// 中文英文标点符号
    /// </summary>
    private static readonly IReadOnlyDictionary<char, char> ChineseEnglishPunctuationDict = new Dictionary<char, char>() {
        {'！', '!'},
        {'（', '('},
        {'）', ')'},
        {'、', ','},
        {'‘', '\''},
        {'’', '\''},
        {'“', '"'},
        {'”', '"'},
        {'；', ';'},
        {'：', ':'},
        {'，', ','},
        {'。', '.'},
        {'？', '?'},
    };

    /// <summary>
    /// 替换英文标点为中文标点
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ReplaceEnglishPunctuationWithChinese(string text) {
        var array = text.ToCharArray();
        for (int i = 0; i < array.Length; i++) {
            if (EnglishChinesePunctuationDict.TryGetValue(array[i], out var ch)) {
                array[i] = ch;
            }
        }
        return new(array);
    }

    /// <summary>
    /// 替换中文标点为英文标点
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ReplaceChinesePunctuationWithEnglish(string text) {
        var array = text.ToCharArray();
        for (int i = 0; i < array.Length; i++) {
            if (ChineseEnglishPunctuationDict.TryGetValue(array[i], out var ch)) {
                array[i] = ch;
            }
        }
        return new(array);
    }

    /// <summary>
    /// 文件替换英文标点为中文标点
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileReplaceEnglishPunctuationWithChinese(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, ReplaceEnglishPunctuationWithChinese);
    }

    /// <summary>
    /// 文件替换中文标点为英文标点
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileReplaceChinesePunctuationWithEnglish(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, ReplaceChinesePunctuationWithEnglish);
    }
}
