using CommonUITools.Utils;
using System;
using System.IO;
using System.Text;

namespace CommonUtil.Core;

public class Base64Tool {
    /// <summary>
    /// base64 字符串解码
    /// </summary>
    /// <param name="encoded"></param>
    /// <param name="encodeType">字符串编码方式</param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="DecoderFallbackException"></exception>
    public static string Base64DecodeString(string encoded, Encoding encodeType)
        => encodeType.GetString(Convert.FromBase64String(encoded));

    /// <summary>
    /// base64 字符串解码
    /// </summary>
    /// <param name="encoded">以 UTF8 编码的字符串</param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="DecoderFallbackException"></exception>
    public static string Base64DecodeString(string encoded)
        => Base64DecodeString(encoded, Encoding.UTF8);

    /// <summary>
    /// base64 字符串编码
    /// </summary>
    /// <param name="source"></param>
    /// <param name="encodeType">编码方式</param>
    /// <returns></returns>
    /// <exception cref="DecoderFallbackException"></exception>
    public static string Base64EncodeString(string source, Encoding encodeType)
        => Convert.ToBase64String(encodeType.GetBytes(source));

    /// <summary>
    /// base64 字符串编码，以 UTF8 进行编码
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="DecoderFallbackException"></exception>
    public static string Base64EncodeString(string source)
        => Base64EncodeString(source, Encoding.UTF8);

    /// <summary>
    /// 文件转base64
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string Base64EncodeFile(string path)
        => Convert.ToBase64String(File.ReadAllBytes(path));

    /// <summary>
    /// base64 转文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] Base64DecodeFile(string path) {
        return Convert.FromBase64String(File.ReadAllText(path));
    }

    /// <summary>
    /// 尝试解码
    /// </summary>
    /// <param name="encoded"></param>
    /// <returns>失败返回 null</returns>
    public static byte[]? TryDecode(string encoded)
        => CommonUtils.Try(() => Convert.FromBase64String(encoded));
}
