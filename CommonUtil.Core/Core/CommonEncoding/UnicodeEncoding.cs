using System.Text.RegularExpressions;

namespace CommonUtil.Core;

public static partial class UnicodeEncoding {
#if NET7_0_OR_GREATER
    private static readonly Regex UnicodeRegex = GetUnicodeRegex();
    [GeneratedRegex("\\\\u(\\w{4})", RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex GetUnicodeRegex();
#elif NET6_0_OR_GREATER
    private static readonly Regex UnicodeRegex = new(@"\\u(\w{4})", RegexOptions.IgnoreCase);
#endif

    /// <summary>
    /// Unicode 编码，新增到 <paramref name="sb"/> 后面
    /// </summary>
    /// <param name="s"></param>
    /// <param name="sb"></param>
    /// <returns>同 <paramref name="sb"/></returns>
    private static StringBuilder UnicodeEncode(string s, StringBuilder sb) {
        byte[] vs = Encoding.Unicode.GetBytes(s);
        for (int i = 0; i < vs.Length >> 1; i++) {
            byte n1 = vs[(i << 1) + 1];
            byte n2 = vs[i << 1];
            // 补全 4 字符
            string s1 = Convert.ToString(n1, 16).PadLeft(2, '0');
            string s2 = Convert.ToString(n2, 16).PadLeft(2, '0');
            sb.Append($"\\u{s1}{s2}");
        }
        return sb;
    }

    /// <summary>
    /// Unicode 编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UnicodeEncode(string s) => UnicodeEncode(s, new()).ToString();

    /// <summary>
    /// Unicode 解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UnicodeDecode(string s) {
        return UnicodeRegex.Replace(s, m => {
            var value = m.Groups[1].Value;
            var s1 = value[..2];
            var s2 = value[2..];
            return Encoding.Unicode.GetString(new byte[] {
                Convert.ToByte(s2, 16),
                Convert.ToByte(s1, 16)
            });
        });
    }

    /// <summary>
    /// Unicode 文件编码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void UnicodeEncodeFile(string inputPath, string outputPath)
        => CommonEncoding.EncodeFileInternal(inputPath, outputPath, UnicodeEncode);

    /// <summary>
    /// Unicode 文件解码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void UnicodeDecodeFile(string inputPath, string outputPath)
        => CommonEncoding.DecodeFileInternal(inputPath, outputPath, UnicodeDecode);
}
