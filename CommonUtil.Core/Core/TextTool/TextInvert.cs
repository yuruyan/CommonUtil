namespace CommonUtil.Core;

public static class TextInvert {
    /// <summary>
    /// 翻转文本
    /// </summary>
    /// <param name="text"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static string InvertText(string text, InversionMode mode) {
        text = text.ReplaceLineFeedWithLinuxStyle();
        return mode switch {
            InversionMode.Horizontal => string.Join('\n', text
                .Split('\n')
                .Select(s => string.Join("", s.Reverse()))
            ),
            InversionMode.Vertical => string.Join('\n', text.Split('\n').Reverse()),
            InversionMode.Both => string.Join("", text.Reverse()),
            _ => InvertText(text, InversionMode.Both),
        };
    }

    /// <summary>
    /// 文件文本翻转
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="mode"></param>
    public static void FileInvertText(string inputPath, string outputPath, InversionMode mode) {
        File.WriteAllText(outputPath, InvertText(File.ReadAllText(inputPath), mode));
    }
}
