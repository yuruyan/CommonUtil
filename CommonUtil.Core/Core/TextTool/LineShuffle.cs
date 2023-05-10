namespace CommonUtil.Core;

public static class LineShuffle {
    /// <summary>
    /// 打乱文本行
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    public static IList<string> ShuffleLines(IEnumerable<string> lines) {
        var data = lines.ToList();
        for (int i = 0, total = data.Count; i < total; i++) {
            var index = Random.Shared.Next(total - i);
            var replaceIndex = total - i - 1;
            (data[index], data[replaceIndex]) = (data[replaceIndex], data[index]);
        }
        return data;
    }

    /// <summary>
    /// 文件打乱文本行
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void FileShuffleLines(string inputPath, string outputPath) {
        File.WriteAllLines(outputPath, ShuffleLines(File.ReadAllLines(inputPath)));
    }
}
