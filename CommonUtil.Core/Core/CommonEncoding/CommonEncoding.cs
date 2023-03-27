namespace CommonUtil.Core;

internal static class CommonEncoding {
    /// <summary>
    /// 文件编码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="func"></param>
    public static void EncodeFileInternal(
        string inputPath,
        string outputPath,
        Func<string, StringBuilder, StringBuilder> func
    ) {
        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);
        var buffer = new char[ConstantUtils.DefaultFileBufferSize];
        int readCount;
        var sb = new StringBuilder(buffer.Length << 3);
        while ((readCount = reader.Read(buffer, 0, buffer.Length)) > 0) {
            writer.Write(func(new(buffer, 0, readCount), sb));
            sb.Clear();
        }
    }

    /// <summary>
    /// 文件解码
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="func"></param>
    public static void DecodeFileInternal(
        string inputPath,
        string outputPath,
        Func<string, string> func
    ) {
        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);
        var buffer = new char[ConstantUtils.DefaultFileBufferSize];
        int readCount;
        int lengthOfEncoded = 8;
        string preDecoded = string.Empty;
        while ((readCount = reader.Read(buffer, 0, buffer.Length)) > 0) {
            var currentDecoded = func(new(buffer, 0, readCount));
            // 前一个解码的后部分
            var preTail = preDecoded[^(Math.Min(preDecoded.Length, lengthOfEncoded))..];
            // 当前解码的前部分
            var currentHead = currentDecoded[..(Math.Min(currentDecoded.Length, lengthOfEncoded))];
            writer.Write(preDecoded[..^(preTail.Length)] + func(preTail + currentHead));
            // 当前解码的后部分
            preDecoded = currentDecoded[currentHead.Length..];
        }
        // 写入最后数据
        writer.Write(preDecoded);
    }
}
