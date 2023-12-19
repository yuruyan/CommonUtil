namespace CommonUtil.Core;

/// <summary>
/// 字典替换文本
/// </summary>
public static class DictionaryReplacement {
    public static string ReplaceAggregate(string text, IReadOnlyDictionary<string, string> replacement) {
        foreach (var dict in replacement) {
            text = text.Replace(dict.Key, dict.Value);
        }
        return text;
    }

    public static void FileReplaceAggregate(string inputPath, string outputPath, IReadOnlyDictionary<string, string> replacement) {
        File.WriteAllText(outputPath, ReplaceAggregate(File.ReadAllText(inputPath), replacement));
    }
}