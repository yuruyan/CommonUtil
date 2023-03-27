namespace CommonUtil.Core;

public static class HexEncoding {
    /// <summary>
    /// Hex 编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string HexEncode(string s) => HexEncode(s, new()).ToString();

    /// <summary>
    /// Hex 编码，新增到 <paramref name="sb"/> 后面
    /// </summary>
    /// <param name="s"></param>
    /// <param name="sb"></param>
    /// <returns>同 <paramref name="sb"/></returns>
    private static StringBuilder HexEncode(string s, StringBuilder sb)
        => sb.Append(Convert.ToHexString(Encoding.UTF8.GetBytes(s)));

    /// <summary>
    /// Hex 文件编码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    public static void HexEncodeFile(string inputPath, string outputPath)
        => CommonEncoding.EncodeFileInternal(inputPath, outputPath, HexEncode);

    /// <summary>
    /// Hex 解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string HexDecode(string s)
        => Encoding.UTF8.GetString(Convert.FromHexString(s));

    /// <summary>
    /// Hex 文件解码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <remarks>数据全部加载进内存， 适合小型文件</remarks>
    public static void HexDecodeFile(string inputPath, string outputPath)
        => File.WriteAllText(outputPath, HexDecode(File.ReadAllText(inputPath)));
}
