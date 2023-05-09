namespace CommonUtil.Core;

public static class DuplicateRemoval {
    /// <summary>
    /// 文本去重
    /// </summary>
    /// <param name="text"></param>
    /// <param name="splitSymbol">分隔符</param>
    /// <param name="mergeSymbol">合并符</param>
    /// <param name="trim">移除元素首尾空白</param>
    /// <returns></returns>
    public static string RemoveDuplicate(string text, string splitSymbol, string mergeSymbol, bool trim = false) {
        //分隔每个字符
        if (splitSymbol == string.Empty) {
            return string.Join(mergeSymbol, text.ToHashSet());
        }
        if (!trim) {
            return string.Join(mergeSymbol, new HashSet<string>(
                text.ReplaceLineFeedWithLinuxStyle().Split(splitSymbol)
            ));
        }
        return string.Join(mergeSymbol, new HashSet<string>(
            text
                .ReplaceLineFeedWithLinuxStyle()
                .Split(splitSymbol, StringSplitOptions.TrimEntries)
        ));
    }

    /// <summary>
    /// 文件文本去重
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="splitSymbol">分隔符</param>
    /// <param name="mergeSymbol">合并符</param>
    /// <param name="trim">移除元素首尾空白</param>
    public static void FileRemoveDuplicate(
        string inputPath,
        string outputPath,
        string splitSymbol,
        string mergeSymbol,
        bool trim = false
    ) {
        File.WriteAllText(
            outputPath,
            RemoveDuplicate(
                File.ReadAllText(inputPath),
                splitSymbol,
                mergeSymbol,
                trim
            )
        );
    }
}
