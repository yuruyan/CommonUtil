using System.Web;

namespace CommonUtil.Core;

public static class UrlEncoding {
    /// <summary>
    /// URL 编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UrlEncode(string s) => HttpUtility.UrlEncode(s);

    /// <summary>
    /// Url 编码，新增到 <paramref name="sb"/> 后面
    /// </summary>
    /// <param name="s"></param>
    /// <param name="sb"></param>
    /// <returns>同 <paramref name="sb"/></returns>
    private static StringBuilder UrlEncode(string s, StringBuilder sb) => sb.Append(UrlEncode(s));

    /// <summary>
    /// Url 文件编码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void UrlEncodeFile(string inputPath, string outputPath)
        => CommonEncoding.EncodeFileInternal(inputPath, outputPath, UrlEncode);

    /// <summary>
    /// URL 解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UrlDecode(string s) {
        return HttpUtility.UrlDecode(s);
    }

    /// <summary>
    /// Url 文件解码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <remarks>数据全部加载进内存， 适合小型文件</remarks>
    public static void UrlDecodeFile(string inputPath, string outputPath)
        => File.WriteAllText(outputPath, UrlDecode(File.ReadAllText(inputPath)));
}
