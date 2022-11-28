using CommonUITools.Utils;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CommonUtil.Core;

public static partial class CommonEncoding {
    private static readonly Regex UTF8Regex = GetUTF8Regex();
    private static readonly Regex UnicodeRegex = GetUnicodeRegex();

    [GeneratedRegex("&#x(\\w{4});", RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex GetUTF8Regex();
    [GeneratedRegex("\\\\u(\\w{4})", RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex GetUnicodeRegex();

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
    public static void UTF8EncodeFile(string inputPath, string outputPath) {
        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);
        var buffer = new char[ConstantUtils.DefaultFileBufferSize];
        int readCount;
        var sb = new StringBuilder(buffer.Length << 3);
        while ((readCount = reader.Read(buffer, 0, buffer.Length)) > 0) {
            writer.Write(UTF8Encode(new(buffer, 0, readCount), sb));
            sb.Clear();
        }
    }

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
    public static void UTF8DecodeFile(string inputPath, string outputPath) {
        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);
        var buffer = new char[ConstantUtils.DefaultFileBufferSize];
        int readCount;
        int lengthOfUTF8 = 8;
        string preDecoded = string.Empty;
        while ((readCount = reader.Read(buffer, 0, buffer.Length)) > 0) {
            var currentDecoded = UTF8Decode(new(buffer, 0, readCount));
            // 前一个解码的后部分
            var preTail = preDecoded[^(Math.Min(preDecoded.Length, lengthOfUTF8))..];
            // 当前解码的前部分
            var currentHead = currentDecoded[..(Math.Min(currentDecoded.Length, lengthOfUTF8))];
            writer.Write(preDecoded[..^(preTail.Length)] + UTF8Decode(preTail + currentHead));
            // 当前解码的后部分
            preDecoded = currentDecoded[currentHead.Length..];
        }
        // 写入最后数据
        writer.Write(preDecoded);
    }

    /// <summary>
    /// Unicode 编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UnicodeEncode(string s) => UnicodeEncode(s, new()).ToString();

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
    /// Unicode 文件编码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void UnicodeEncodeFile(string inputPath, string outputPath) {
        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);
        var buffer = new char[ConstantUtils.DefaultFileBufferSize];
        int readCount;
        var sb = new StringBuilder(buffer.Length << 3);
        while ((readCount = reader.Read(buffer, 0, buffer.Length)) > 0) {
            writer.Write(UnicodeEncode(new(buffer, 0, readCount), sb));
            sb.Clear();
        }
    }

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
    /// Unicode 文件解码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void UnicodeDecodeFile(string inputPath, string outputPath) {
        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);
        var buffer = new char[ConstantUtils.DefaultFileBufferSize];
        int readCount;
        int lengthOfUnicode = 8;
        string preDecoded = string.Empty;
        while ((readCount = reader.Read(buffer, 0, buffer.Length)) > 0) {
            var currentDecoded = UnicodeDecode(new(buffer, 0, readCount));
            // 前一个解码的后部分
            var preTail = preDecoded[^(Math.Min(preDecoded.Length, lengthOfUnicode))..];
            // 当前解码的前部分
            var currentHead = currentDecoded[..(Math.Min(currentDecoded.Length, lengthOfUnicode))];
            writer.Write(preDecoded[..^(preTail.Length)] + UnicodeDecode(preTail + currentHead));
            // 当前解码的后部分
            preDecoded = currentDecoded[currentHead.Length..];
        }
        // 写入最后数据
        writer.Write(preDecoded);
    }

    /// <summary>
    /// URL 编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UrlEncode(string s) {
        return HttpUtility.UrlEncode(s);
    }

    /// <summary>
    /// URL 解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UrlDecode(string s) {
        return HttpUtility.UrlDecode(s);
    }

    /// <summary>
    /// Hex 编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string HexEncode(string s) {
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        var sb = new StringBuilder(bytes.Length << 1);
        foreach (var b in bytes) {
            sb.Append(Convert.ToString(b, 16));
        }
        return sb.ToString();
    }

    /// <summary>
    /// Hex 解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string HexDecode(string s) {
        s = s.Trim();
        byte[] data = new byte[s.Length >> 1];
        for (int i = 0; i < s.Length; i += 2) {
            data[i >> 1] = Convert.FromHexString($"{s[i]}{s[i + 1]}")[0];
        }
        return Encoding.UTF8.GetString(data);
    }

}
