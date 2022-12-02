namespace SimpleFileSystemServer.Service;

public static class FileService {
    /// <summary>
    /// 获取目录下的文件信息
    /// </summary>
    /// <param name="dirPath">绝对路径，以'/'开头</param>
    /// <returns>失败返回空列表</returns>
    public static IList<FileVO> ListFiles(string dirPath) {
        var directoryInfo = new DirectoryInfo(PathUtils.GetAbsolutePath(dirPath));
        // 不在根目录范围内
        if (!PathUtils.CheckPathRange(directoryInfo.FullName)) {
            return new List<FileVO>();
        }
        // 目录不存在
        if (!directoryInfo.Exists) {
            return new List<FileVO>();
        }

        var results = new List<FileVO>();
        // 获取目录
        results.AddRange(directoryInfo
            .GetDirectories()
            .Select(f => new FileVO(f.Name, dirPath, true))
        );
        // 获取文件
        results.AddRange(directoryInfo
            .GetFiles()
            .Select(f => new FileVO(f.Name, dirPath, false) {
                FileSize = f.Length,
            })
        );
        return results;
    }

}
