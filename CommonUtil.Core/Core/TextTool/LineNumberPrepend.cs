namespace CommonUtil.Core;

public static class LineNumberPrepend {
    /// <summary>
    /// 添加行号
    /// </summary>
    /// <param name="text"></param>
    /// <param name="separator">数字与文本分隔符</param>
    /// <returns></returns>
    public static string PrependLineNumber(string text, string separator) {
        string[] lines = text.ReplaceLineFeedWithLinuxStyle().Split('\n');
        for (int i = 0; i < lines.Length; i++) {
            lines[i] = $"{i + 1}{separator}" + lines[i];
        }
        return string.Join('\n', lines);
    }

    /// <summary>
    /// 文件文本添加行号
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="separator">数字与文本分隔符</param>
    /// <returns></returns>
    public static void FilePrependLineNumber(string inputPath, string outputPath, string separator) {
        File.WriteAllText(outputPath, PrependLineNumber(File.ReadAllText(inputPath), separator));
    }
}
