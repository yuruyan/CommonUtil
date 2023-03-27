namespace CommonUtil.Core;

public static class Base64Encoding {
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
    /// <remarks>文件会全部加载进内存，适合小文件</remarks>
    public static string Base64EncodeFile(string path)
        => Convert.ToBase64String(File.ReadAllBytes(path));

    /// <summary>
    /// base64 转文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <remarks>文件会全部加载进内存，适合小文件</remarks>
    public static byte[] Base64DecodeFile(string path) {
        return Convert.FromBase64String(File.ReadAllText(path));
    }

    /// <summary>
    /// 编码文件
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="outputFile"></param>
    /// <param name="token"></param>
    /// <param name="callback">回调</param>
    /// <returns>编码后文件长度，任务取消返回 0</returns>
    public static long Base64EncodeFile(string inputFile, string outputFile, CancellationToken? token = null, Action<double>? callback = null) {
        using var readStream = File.OpenRead(inputFile);
        using var writeStream = new StreamWriter(new FileStream(outputFile, FileMode.Create, FileAccess.Write));
        // 需要是 3 倍
        var buffer = new byte[1024 * 3];
        var outBuffer = new char[4096];
        long totalLength = readStream.Length, totalReadCount = 0;
        int readCount;
        // 分批读取
        while ((readCount = readStream.Read(buffer)) > 0) {
            // 中断
            if (token?.IsCancellationRequested == true) {
                return 0;
            }
            totalReadCount += readCount;
            callback?.Invoke((double)totalReadCount / totalLength);
            int outBufferCount = Convert.ToBase64CharArray(buffer, 0, readCount, outBuffer, 0);
            writeStream.Write(outBuffer, 0, outBufferCount);
        }
        callback?.Invoke(1);
        writeStream.Flush();
        return writeStream.BaseStream.Length;
    }

    /// <summary>
    /// 解码文件
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="outputFile"></param>
    /// <param name="token"></param>
    /// <param name="callback">回调</param>
    /// <returns>解码后文件长度，任务取消返回 0</returns>
    public static long Base64DecodeFile(string inputFile, string outputFile, CancellationToken? token = null, Action<double>? callback = null) {
        using var readStream = File.OpenRead(inputFile);
        using var writeStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
        // 需要是 4 倍
        var buffer = new byte[1024 * 4];
        long totalLength = readStream.Length, totalReadCount = 0;
        int readCount;
        // 分批读取
        while ((readCount = readStream.Read(buffer, 0, buffer.Length)) > 0) {
            // 中断
            if (token?.IsCancellationRequested == true) {
                return 0;
            }
            totalReadCount += readCount;
            callback?.Invoke((double)totalReadCount / totalLength);
            var outData = Convert.FromBase64String(Encoding.ASCII.GetString(buffer, 0, readCount));
            writeStream.Write(outData);
        }
        callback?.Invoke(1);
        writeStream.Flush();
        return writeStream.Length;
    }

    /// <summary>
    /// 尝试解码
    /// </summary>
    /// <param name="encoded"></param>
    /// <returns>失败返回 null</returns>
    public static byte[]? TryDecode(string encoded)
        => TaskUtils.Try(() => Convert.FromBase64String(encoded));
}
