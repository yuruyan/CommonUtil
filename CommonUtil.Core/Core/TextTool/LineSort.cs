namespace CommonUtil.Core;

public static class LineSort {
    /// <summary>
    /// 排序文本行
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string SortLines(string text) {
        var lines = text.Split('\n');
        Array.Sort(lines);
        return string.Join('\n', lines);
    }

    /// <summary>
    /// 文件排序文本行
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileSortLines(string inputPath, string outputPath) {
        TextTool.ProcessFileText(inputPath, outputPath, SortLines);
    }
}
