using System;
using System.IO;

namespace CommonUtil.Core;

public class FileMergeSplit {
    public class ProcessMonitor {
        public double Process { get; set; } = 0;
        public string ProcessingName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 分割文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="saveDir">保存目录</param>
    /// <param name="perSize">每个文件的大小</param>
    /// <param name="processMonitor">进度监控</param>
    public static void SplitFile(string filePath, string saveDir, long perSize, ProcessMonitor? processMonitor) {
        var monitor = processMonitor ?? new ProcessMonitor();
        int fileCount = 1; // 文件数目
        int fileSizeCount = 0; // 文件大小计数
        const int bufferSize = 0x400; // 缓冲区大小
        var fileInfo = new FileInfo(filePath);
        var filename = fileInfo.Name[..^fileInfo.Extension.Length]; // 文件名
        var extension = fileInfo.Extension; // 扩展名
        var totalFileCount = (int)Math.Ceiling((double)fileInfo.Length / perSize); // 分割文件总数
        saveDir = new DirectoryInfo(saveDir).FullName; // 保存文件夹

        var reader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));
        var writer = new BinaryWriter(new FileStream(Path.Combine(saveDir, $"{filename}-{fileCount}{extension}"), FileMode.OpenOrCreate, FileAccess.Write));
        var buffer = new byte[bufferSize];
        int readCount = 0;
        monitor.Process = 0;
        while ((readCount = reader.Read(buffer, 0, bufferSize)) != 0) {
            writer.Write(buffer, 0, readCount);
            fileSizeCount += readCount;
            // 写完一个文件
            if (fileSizeCount >= perSize) {
                writer.Close();
                fileSizeCount = 0;
                fileCount++;
                monitor.Process = (double)fileCount / totalFileCount;
                // 打开新文件
                writer = new BinaryWriter(new FileStream(Path.Combine(saveDir, $"{filename}-{fileCount}{extension}"), FileMode.OpenOrCreate, FileAccess.Write));
            }
        }
        monitor.Process = 1;
        writer.Close();
        reader.Close();
    }

    /// <summary>
    /// 合并文件
    /// </summary>
    /// <param name="sourceFiles">待合并的文件所在目录</param>
    /// <param name="savePath">保存文件路径</param>
    /// <param name="processMonitor">进度监控</param>
    public static void MergeFile(string[] sourceFiles, string savePath, ProcessMonitor? processMonitor) {
        var monitor = processMonitor ?? new ProcessMonitor();
        const int bufferSize = 0x100000; // 缓冲区大小
        int finishedCount = 0; // 完成文件数目
        var buffer = new byte[bufferSize];
        int readCount = 0;
        monitor.Process = 0;
        var writer = new BinaryWriter(new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write));
        foreach (var file in sourceFiles) {
            var reader = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read));
            while ((readCount = reader.Read(buffer, 0, bufferSize)) != 0) {
                writer.Write(buffer, 0, readCount);
            }
            reader.Close();
            finishedCount++;
            monitor.Process = (double)finishedCount / sourceFiles.Length;
        }
        monitor.Process = 1;
        writer.Close();
    }

}
