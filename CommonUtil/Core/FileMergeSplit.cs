using System;
using System.IO;
using System.Linq;

namespace CommonUtil.Core;

public class FileMergeSplit {
    private const uint BufferSize = 4096 << 2;

    /// <summary>
    /// 分割文件
    /// </summary>
    /// <param name="filePath">文件绝对路径</param>
    /// <param name="saveDir">保存文件夹绝对路径</param>
    /// <param name="perFileSize">每一个分割后的文件大小</param>
    /// <param name="processCallback">进度回调，参数为进度[0, 100]</param>
    public static void SplitFile(
        string filePath,
        string saveDir,
        ulong perFileSize,
        Action<double>? processCallback = null
    ) {
        uint currentFileCount = 1; // 文件数目
        ulong fileSizeCount = 0; // 文件大小计数
        var fileInfo = new FileInfo(filePath);
        var filename = fileInfo.Name[..^fileInfo.Extension.Length]; // 文件名，不包括后缀
        var extension = fileInfo.Extension; // 扩展名
        uint totalFileCount = (uint)Math.Ceiling((double)fileInfo.Length / perFileSize); // 分割文件总数
        using var reader = new BinaryReader(File.OpenRead(filePath));
        var writer = new BinaryWriter(File.OpenWrite(GetSplitFilePath(
            saveDir, filename, extension, currentFileCount, totalFileCount
        )));
        var buffer = new byte[Math.Min(BufferSize, perFileSize)];
        int readCount = 0;
        long totalReadCount = 0; // 总已读字节数
        while ((readCount = reader.Read(buffer, 0, buffer.Length)) != 0) {
            writer.Write(buffer, 0, readCount);
            fileSizeCount += (uint)readCount;
            totalReadCount += readCount;
            processCallback?.Invoke((double)totalReadCount * 100 / fileInfo.Length);
            // 写完一个文件
            if (fileSizeCount >= perFileSize) {
                writer.Close();
                fileSizeCount = 0;
                currentFileCount++;
                // 打开新文件
                writer = new BinaryWriter(File.OpenWrite(GetSplitFilePath(
                    saveDir, filename, extension, currentFileCount, totalFileCount
                )));
            }
        }
        writer.Close();
    }

    /// <summary>
    /// 获取分割文件绝对路径
    /// </summary>
    /// <param name="saveDir">保存文件夹</param>
    /// <param name="filename">文件名，不包括后缀</param>
    /// <param name="extension">文件后缀，包括.</param>
    /// <param name="currentCount">当前文件索引位置</param>
    /// <param name="totalCount">总分割文件</param>
    /// <returns></returns>
    private static string GetSplitFilePath(
        string saveDir,
        string filename,
        string extension,
        uint currentCount,
        uint totalCount
    )
        => Path.Combine(
            saveDir,
            $"{filename}-{currentCount.ToString().PadLeft(totalCount.ToString().Length, '0')}{extension}"
        );

    /// <summary>
    /// 合并文件
    /// </summary>
    /// <param name="sourceFiles">分割文件绝对路径列表</param>
    /// <param name="savePath">保存文件</param>
    /// <param name="processCallback">进度回调，参数为进度[0, 100]</param>
    public static void MergeFile(
        string[] sourceFiles,
        string savePath,
        Action<double>? processCallback = null
    ) {
        int finishedCount = 0; // 完成文件数目
        var buffer = new byte[BufferSize];
        int readCount = 0;
        long totalReadSize = 0; // 总已读字节数
        // 文件总大小
        long totalFileSize = sourceFiles
            .Select(f => new FileInfo(f).Length)
            .Sum();
        var writer = new BinaryWriter(File.OpenWrite(savePath));
        foreach (var file in sourceFiles) {
            using var reader = new BinaryReader(File.OpenRead(file));
            while ((readCount = reader.Read(buffer, 0, buffer.Length)) != 0) {
                writer.Write(buffer, 0, readCount);
                totalReadSize += readCount;
                processCallback?.Invoke((double)totalReadSize * 100 / totalFileSize);
            }
            finishedCount++;
        }
        writer.Close();
    }
}
