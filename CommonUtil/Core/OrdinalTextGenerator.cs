namespace CommonUtil.Core;

public class OrdinalTextGenerator {
    private const string LeftBracketReplacement = "\0\u0001\u0020\u0300\u1234\uffff\u1234\u0300\u0020\u0001\0";
    private const string RightBracketReplacement = "\uffff\u0001\u0020\u0300\u1234\0\u1234\u0300\u0020\u0001\uffff";

    /// <summary>
    /// 生成顺序文本
    /// </summary>
    /// <param name="format"></param>
    /// <param name="startIndex"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string[] Generate(string format, int startIndex, uint count) {
        format = format.Replace("{{", LeftBracketReplacement)
            .Replace("}}", RightBracketReplacement)
            .Replace("{}", "{0}")
            .Replace(LeftBracketReplacement, "{{")
            .Replace(RightBracketReplacement, "}}");
        string[] data = new string[count];
        for (int i = 0; i < data.Length; i++) {
            data[i] = string.Format(format, i + startIndex);
        }
        return data;
    }
}

