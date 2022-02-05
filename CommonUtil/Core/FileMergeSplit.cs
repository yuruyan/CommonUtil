using System.IO;

namespace CommonUtil.Core {
    public class FileMergeSplit {
        /// <summary>
        /// 分割文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="saveDir">保存目录</param>
        /// <param name="perSize">每个文件的大小</param>
        public static void SplitFile(string filePath, string saveDir, long perSize) {
            int fileCount = 1; // 文件数目
            int fileSizeCount = 0; // 文件大小计数
            const int bufferSize = 0x400; // 缓冲区大小
            var fileInfo = new FileInfo(filePath);
            var filename = fileInfo.Name[..^fileInfo.Extension.Length]; // 文件名
            var extension = fileInfo.Extension; // 扩展名
            saveDir = new DirectoryInfo(saveDir).FullName;

            var reader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));
            var writer = new BinaryWriter(new FileStream($"{saveDir}/{filename}-{fileCount}{extension}", FileMode.OpenOrCreate, FileAccess.Write));
            var buffer = new byte[bufferSize];
            int readCount = 0;
            while ((readCount = reader.Read(buffer, 0, bufferSize)) != 0) {
                writer.Write(buffer, 0, readCount);
                fileSizeCount += readCount;
                // 写完一个文件
                if (fileSizeCount >= perSize) {
                    writer.Close();
                    fileSizeCount = 0;
                    fileCount++;
                    // 打开新文件
                    writer = new BinaryWriter(new FileStream($"{saveDir}/{filename}-{fileCount}{extension}", FileMode.OpenOrCreate, FileAccess.Write));
                }
            }
            writer.Close();
            reader.Close();
        }

        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="sourceFiles">待合并的文件所在目录</param>
        /// <param name="savePath">保存文件路径</param>
        public static void MergeFile(string[] sourceFiles, string savePath) {
            const int bufferSize = 0x100000; // 缓冲区大小
            int finishedCount = 0; // 完成文件数目
            var buffer = new byte[bufferSize];
            int readCount = 0;
            var writer = new BinaryWriter(new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write));
            foreach (var file in sourceFiles) {
                var reader = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read));
                while ((readCount = reader.Read(buffer, 0, bufferSize)) != 0) {
                    writer.Write(buffer, 0, readCount);
                }
                reader.Close();
                finishedCount++;
            }
            writer.Close();
        }

    }

}
