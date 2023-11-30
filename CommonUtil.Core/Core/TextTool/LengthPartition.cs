namespace CommonUtil.Core;

public static class LengthPartition {
    /// <summary>
    /// 根据长度分割字符串
    /// </summary>
    /// <param name="text">源文本</param>
    /// <param name="length">每段字符串长度</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static List<string> Split(string text, int length) {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
#else
        if (length <= 0) {
            throw new ArgumentOutOfRangeException(nameof(length));
        }
#endif

        var textLength = text.Length;
        var results = new List<string>(textLength / length);
        int i = 0;
        for (i = 0; i + length < textLength; i += length) {
            results.Add(text[i..(i + length)]);
        }
        if (i < textLength) {
            results.Add(text[i..]);
        }
        return results;
    }

    /// <summary>
    /// 文件文本处理
    /// </summary>
    /// <param name="inputPath">源文件</param>
    /// <param name="outputPath">输出文件</param>
    /// <inheritdoc cref="Split(string, int)"/>
    public static void FileSplit(string inputPath, string outputPath, int length) {
        File.WriteAllLines(outputPath, Split(File.ReadAllText(inputPath), length));
    }
}
