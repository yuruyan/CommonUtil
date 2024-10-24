namespace CommonUtil.Core;

public static class HtmlTableConversion {
    public static string ConvertToTable(string text, string separator) {
        var rows = text.ReplaceLineFeedWithLinuxStyle().Split('\n');
        var newRows = new List<string>(rows.Length);
        var splitList = rows
            .Where(row => !string.IsNullOrWhiteSpace(row))
            .Select(row => row.Split(separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .ToList();
        var maxColumnCount = splitList.Max(line => line.Length);
        // 统一列数
        splitList = splitList.Select(
            line => line
                .Select(EscapeCharacters)
                .ToList()
                .ResizeToLength(maxColumnCount, string.Empty)
                .ToArray()
        ).ToList();
        var sb = new StringBuilder();
        sb.Append("<table>").Append("<thead>").Append("<tr>");
        for (int i = 0; i < maxColumnCount; i++) {
            sb.Append("<th>").Append(splitList[0][i]).Append("</th>");
        }
        sb.Append("</tr>").Append("</thead>").Append("<tbody>");

        for (int i = 1; i < splitList.Count; i++) {
            sb.Append("<tr>");
            foreach (var item in splitList[i]) {
                sb.Append("<td>").Append(item).Append("</td>");
            }
            sb.Append("</tr>");
        }
        sb.Append("</tbody>").Append("</table>");
        return sb.ToString();
    }

    /// <summary>
    /// 转义字符
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static string EscapeCharacters(string text) {
        return text
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("&", "&amp;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}
