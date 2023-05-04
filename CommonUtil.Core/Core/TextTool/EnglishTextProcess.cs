namespace CommonUtil.Core;

public static class EnglishTextProcess {
    #region Field
    /// <summary>
    /// 半角全角 Dict
    /// </summary>
    private static readonly IReadOnlyDictionary<char, char> HalfFullCharDict;
    /// <summary>
    /// 全角半角 Dict
    /// </summary>
    private static readonly IReadOnlyDictionary<char, char> FullHalfCharDict;
    #endregion

    static EnglishTextProcess() {
        var halfFullCharDict = new Dictionary<char, char>();
        var fullHalfCharDict = new Dictionary<char, char>();
        // 空格
        halfFullCharDict[(char)32u] = (char)12288;
        // 其余字符
        for (char i = (char)33; i < 127; i++) {
            halfFullCharDict[i] = (char)(i + 65248);
        }
        // 填充 FullHalfCharDict
        foreach (var item in halfFullCharDict) {
            fullHalfCharDict[item.Value] = item.Key;
        }
        HalfFullCharDict = halfFullCharDict;
        FullHalfCharDict = fullHalfCharDict;
    }

    #region TextProcess
    /// <summary>
    /// 小写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToLowerCase(string text) => text.ToLowerInvariant();

    /// <summary>
    /// 大写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToUpperCase(string text) => text.ToUpperInvariant();

    /// <summary>
    /// 切换大小写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToggleCase(string text) {
        char[] chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++) {
            char ch = chars[i];
            chars[i] = ch switch {
                >= 'a' and <= 'z' => char.ToUpper(ch),
                >= 'A' and <= 'Z' => char.ToLower(ch),
                _ => ch
            };
        }
        return new(chars);
    }

    /// <summary>
    /// 将每个单词首字母大写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string CapitalizeWords(string text)
        => TextTool.EnglishWordRegex.Replace(
            ToLowerCase(text),
            match => CapitalizeFirstWordCharacter(match.Value)
        );

    /// <summary>
    /// 将文本第一个字母大写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static string CapitalizeFirstWordCharacter(string text) {
        var chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++) {
            char ch = chars[i];
            if (char.IsLetter(ch)) {
                chars[i] = char.ToUpper(ch);
                break;
            }
        }
        return new(chars);
    }

    /// <summary>
    /// 将每一句话首字母大写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToSentenceCase(string text) {
        var lines = ToLowerCase(text).Split('\n');
        // 对每一行根据 EnglishSentenceSeparator 分割
        for (int i = 0; i < lines.Length; i++) {
            var sentences = lines[i].Split(TextTool.EnglishSentenceSeparator);
            for (int j = 0; j < sentences.Length; j++) {
                sentences[j] = CapitalizeFirstWordCharacter(sentences[j]);
            }
            lines[i] = string.Join(TextTool.EnglishSentenceSeparator, sentences);
        }
        return string.Join('\n', lines);
    }

    /// <summary>
    /// 半角转全角
    /// </summary>
    /// <param name="halfCharText"></param>
    /// <returns></returns>
    public static string HalfCharToFullChar(string halfCharText) {
        char[] array = halfCharText.ToCharArray();
        for (int i = 0; i < array.Length; i++) {
            if (HalfFullCharDict.ContainsKey(array[i])) {
                array[i] = HalfFullCharDict[array[i]];
            }
        }
        return new(array);
    }

    /// <summary>
    /// 全角转半角
    /// </summary>
    /// <param name="fullCharText"></param>
    /// <returns></returns>
    public static string FullCharToHalfChar(string fullCharText) {
        char[] array = fullCharText.ToCharArray();
        for (int i = 0; i < array.Length; i++) {
            if (FullHalfCharDict.ContainsKey(array[i])) {
                array[i] = FullHalfCharDict[array[i]];
            }
        }
        return new(array);
    }
    #endregion

    #region FileProcess
    /// <summary>
    /// 文件文本半角转全角
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileHalfCharToFullChar(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, HalfCharToFullChar);
    }

    /// <summary>
    /// 文件文本全角转半角
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileFullCharToHalfChar(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, FullCharToHalfChar);
    }

    /// <summary>
    /// 文件文本小写
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileToLowerCase(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, ToLowerCase);
    }

    /// <summary>
    /// 文件文本大写
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileToUpperCase(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, ToUpperCase);
    }

    /// <summary>
    /// 文件文本切换大小写
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileToggleCase(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, ToggleCase);
    }

    /// <summary>
    /// 文件文本将每个单词首字母大写
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileCapitalizeWords(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, CapitalizeWords);
    }

    /// <summary>
    /// 文件文本将每一句话首字母大写
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileToSentenceCase(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, ToSentenceCase);
    }
    #endregion
}
