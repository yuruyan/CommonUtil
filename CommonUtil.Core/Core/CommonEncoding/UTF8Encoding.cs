using System.Text.RegularExpressions;

namespace CommonUtil.Core;

public static partial class UTF8Encoding {

#if NET7_0_OR_GREATER
    private static readonly Regex UTF8Regex = GetUTF8Regex();
    [GeneratedRegex("&#x(\\w{4});", RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex GetUTF8Regex();
#elif NET6_0_OR_GREATER
    private static readonly Regex UTF8Regex = new(@"&#x(\w{4});", RegexOptions.IgnoreCase);
#endif

    /// <summary>
    /// UTF8 编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UTF8Encode(string s) => UTF8Encode(s, new()).ToString();

    /// <summary>
    /// UTF8 编码，新增到 <paramref name="sb"/> 后面
    /// </summary>
    /// <param name="s"></param>
    /// <param name="sb"></param>
    /// <returns>同 <paramref name="sb"/></returns>
    private static StringBuilder UTF8Encode(string s, StringBuilder sb) {
        byte[] vs = Encoding.Unicode.GetBytes(s);
        for (int i = 0; i < vs.Length >> 1; i++) {
            byte n1 = vs[(i << 1) + 1];
            byte n2 = vs[i << 1];
            // ASCII 字符
            if (n1 == 0) {
                sb.Append(s[i]);
                continue;
            }
            // 补全 4 字符
            string s1 = Convert.ToString(n1, 16).PadLeft(2, '0');
            string s2 = Convert.ToString(n2, 16).PadLeft(2, '0');
            sb.Append($"&#x{s1}{s2};");
        }
        return sb;
    }

    /// <summary>
    /// UTF8 文件编码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void UTF8EncodeFile(string inputPath, string outputPath)
        => CommonEncoding.EncodeFileInternal(inputPath, outputPath, UTF8Encode);

    /// <summary>
    /// UTF8 解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UTF8Decode(string s) {
        return UTF8Regex.Replace(s, m => {
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
    /// UTF8 文件解码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void UTF8DecodeFile(string inputPath, string outputPath)
        => CommonEncoding.DecodeFileInternal(inputPath, outputPath, UTF8Decode);
}
