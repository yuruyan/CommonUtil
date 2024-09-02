namespace CommonUtil.Core;

public static class MarkdownTableConversion {
    /// <summary>
    /// 转换为 Markdown 表格
    /// </summary>
    /// <param name="text"></param>
    /// <param name="separator">列分隔符</param>
    /// <returns></returns>
    public static string ConvertToTable(string text, string separator) {
        var rows = text.ReplaceLineFeedWithLinuxStyle().Split('\n');
        var newRows = new List<string>(rows.Length);
        var splitList = rows
            .Where(row => !string.IsNullOrWhiteSpace(row))
            .Select(row => row.Split(separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .ToList();
        var maxColumnCount = splitList.Max(line => line.Length);
        foreach (var row in splitList) {
            // 补足长度
            var content = string.Join(
                '|',
                row.Select(EscapeCharacters).ToList().ResizeToLength(maxColumnCount, string.Empty)
            );
            newRows.Add($"|{content}|");
        }
        // Add seperator between header and content
        if (newRows.Count > 1) {
            var content = string.Join(
                '|',
                new List<string>(0)
                    .ResizeByCount(maxColumnCount, "-")
            );
            newRows.Insert(1, $"|{content}|");
        }
        return string.Join('\n', newRows);
    }

    /// <summary>
    /// 转义Markdown字符
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static string EscapeCharacters(string text) {
        return text
            .Replace("\\", "\\\\")
            .Replace("`", "\\`")
            .Replace("|", "\\|")
            .Replace("*", "\\*")
            .Replace("_", "\\_")
            .Replace("{", "\\{")
            .Replace("}", "\\}")
            .Replace("[", "\\[")
            .Replace("]", "\\]")
            .Replace("(", "\\(")
            .Replace(")", "\\)")
            .Replace("#", "\\#")
            .Replace("+", "\\+")
            .Replace("-", "\\-")
            .Replace(".", "\\.")
            .Replace("!", "\\!");
    }
}
