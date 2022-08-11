using System;
using System.IO;
using System.Text;

namespace CommonUtil.Core;

class Base64Tool {
    /// <summary>
    /// base64 解码
    /// </summary>
    /// <param name="encoded"></param>
    /// <param name="encodeType">字符串编码方式</param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="DecoderFallbackException"></exception>
    public static string Base64StringDecode(string encoded, Encoding encodeType) {
        return encodeType.GetString(Convert.FromBase64String(encoded));
    }

    /// <summary>
    /// base64 解码
    /// </summary>
    /// <param name="encoded">以 UTF8 编码的字符串</param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="DecoderFallbackException"></exception>
    public static string Base64StringDecode(string encoded) {
        return Base64StringDecode(encoded, Encoding.UTF8);
    }

    /// <summary>
    /// base64 编码
    /// </summary>
    /// <param name="source"></param>
    /// <param name="encodeType">编码方式</param>
    /// <returns></returns>
    /// <exception cref="DecoderFallbackException"></exception>
    public static string Base64StringEncode(string source, Encoding encodeType) {
        return Convert.ToBase64String(encodeType.GetBytes(source));
    }

    /// <summary>
    /// base64 编码，以 UTF8 进行编码
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="DecoderFallbackException"></exception>
    public static string Base64StringEncode(string source) {
        return Base64StringEncode(source, Encoding.UTF8);
    }

    /// <summary>
    /// 文件转base64
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="IOException"></exception>
    public static string Base64Encode(string path) {
        return Convert.ToBase64String(File.ReadAllBytes(path));
    }

    /// <summary>
    /// base64 转文件
    /// </summary>
    /// <param name="encoded"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="IOException"></exception>
    /// <exception cref="FormatException"></exception>
    public static void Base64Decode(string encoded, string path) {
        File.WriteAllBytes(path, Convert.FromBase64String(encoded));
    }

    /// <summary>
    /// 尝试解码
    /// </summary>
    /// <param name="encoded"></param>
    /// <returns>失败返回 null</returns>
    public static byte[]? TryDecode(string encoded) {
        try {
            return Convert.FromBase64String(encoded);
        } catch {
            return null;
        }
    }

}

