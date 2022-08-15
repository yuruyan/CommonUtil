using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommonUtil.Core;

public class FileMergeSplit {
    private const uint BufferSize = 4096 << 2;
    private const byte MaxWorkingThreadCount = 4;
    private const ulong MultiThreadFileSizeThreshold = 512 * 1024 * 1024;

    /// <summary>
    /// 分割文件
    /// </summary>
    /// <param name="filePath">文件绝对路径</param>
    /// <param name="saveDir">保存文件夹绝对路径</param>
    /// <param name="perFileSize">每一个分割后的文件大小，最小为 1kb</param>
    /// <param name="processCallback">进度回调，参数为进度[0, 100]</param>
    public static void SplitFile(
        string filePath,
        string saveDir,
        ulong perFileSize,
        Action<double>? processCallback = null
    ) {
        // 每个文件大小不能小于 1kb
        perFileSize = Math.Max(perFileSize, 1024);
        ulong totalReadBytesCount = 0; // 已读取文件大小计数
        var fileInfo = new FileInfo(filePath);
        uint totalFileCount = (uint)Math.Ceiling((double)fileInfo.Length / perFileSize); // 分割文件总数
        // 单线程
        if (totalFileCount == 1
            || (ulong)fileInfo.Length <= MultiThreadFileSizeThreshold && totalFileCount <= MaxWorkingThreadCount
        ) {
            StartSplitFile(
                fileInfo,
                saveDir,
                0,
                (ulong)fileInfo.Length,
                perFileSize,
                1,
                totalFileCount,
                ref totalReadBytesCount,
                processCallback
            );
            return;
        }
        // 多线程
        Task[] tasks = new Task[Math.Min(MaxWorkingThreadCount, totalFileCount)];
        // 每个线程需要读取的文件字节数
        ulong perTaskFileSize = (ulong)Math.Ceiling((double)fileInfo.Length / tasks.Length);
        // 每个线程需要写入的文件总数
        uint perTaskFileCount = (uint)Math.Ceiling((double)perTaskFileSize / perFileSize);
        totalFileCount = (uint)(perTaskFileCount * tasks.Length);
        for (int i = 0; i < tasks.Length; i++) {
            uint tempIndex = (uint)i;
            Task task = Task.Run(() => {
                StartSplitFile(
                    fileInfo,
                    saveDir,
                    perTaskFileSize * tempIndex,
                    perTaskFileSize,
                    perFileSize,
                    perTaskFileCount * tempIndex + 1,
                    totalFileCount,
                    ref totalReadBytesCount,
                    processCallback
                );
            });
            tasks[i] = task;
        }
        Task.WaitAll(tasks);
    }

    /// <summary>
    /// 开始分割文件
    /// </summary>
    /// <param name="splitFileInfo">要分割的文件信息</param>
    /// <param name="saveDir">保存目录</param>
    /// <param name="startReadPosition">起始文件读取位置</param>
    /// <param name="toBeReadSize">需要读取的文件大小</param>
    /// <param name="perFileSize">每个分割文件的大小</param>
    /// <param name="startFileCount">分割文件开始下标</param>
    /// <param name="totalFileCount">总分割文件个数</param>
    /// <param name="totalReadBytesCount">已读取的文件累计大小</param>
    /// <param name="processCallback">进度回调</param>
    /// <returns></returns>
    private static void StartSplitFile(
        FileInfo splitFileInfo,
        string saveDir,
        ulong startReadPosition,
        ulong toBeReadSize,
        ulong perFileSize,
        uint startFileCount,
        uint totalFileCount,
        ref ulong totalReadBytesCount,
        Action<double>? processCallback = null
    ) {
        var extension = splitFileInfo.Extension; // 扩展名
        var filename = splitFileInfo.Name[..^extension.Length]; // 文件名，不包括后缀
        using var reader = new BinaryReader(File.OpenRead(splitFileInfo.FullName));
        reader.BaseStream.Seek((long)startReadPosition, SeekOrigin.Begin);
        var writer = new BinaryWriter(File.OpenWrite(GetSplitFilePath(
            saveDir, filename, extension, startFileCount, totalFileCount
        )));
        var buffer = new byte[Math.Min(BufferSize, perFileSize)];
        ulong fileSizeCount = 0; // 文件大小计数
        ulong totalReadCount = 0; // 本次读取的文件总大小
        int readCount = 0;
        while ((readCount = reader.Read(buffer, 0, buffer.Length)) != 0) {
            ulong longReadCount = (ulong)readCount;
            ulong currentTotalReadCount = totalReadCount + longReadCount;
            // 本次任务读取完毕
            if (currentTotalReadCount >= toBeReadSize) {
                // 读取前半部分
                ulong lastChunckSize = currentTotalReadCount == toBeReadSize ? longReadCount : (ulong)buffer.Length - (currentTotalReadCount - toBeReadSize);
                writer.Write(buffer, 0, (int)lastChunckSize);
                Interlocked.Add(ref totalReadBytesCount, lastChunckSize);
                processCallback?.Invoke((double)totalReadBytesCount * 100 / splitFileInfo.Length);
                writer.Close();
                return;
            }
            writer.Write(buffer, 0, readCount);
            fileSizeCount += longReadCount;
            totalReadCount += longReadCount;
            Interlocked.Add(ref totalReadBytesCount, longReadCount);
            processCallback?.Invoke((double)totalReadBytesCount * 100 / splitFileInfo.Length);
            // 写完一个文件
            if (fileSizeCount >= perFileSize) {
                writer.Close();
                fileSizeCount = 0;
                startFileCount++;
                // 打开新文件
                writer = new BinaryWriter(File.OpenWrite(GetSplitFilePath(
                    saveDir, filename, extension, startFileCount, totalFileCount
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
        File.Delete(savePath);
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
