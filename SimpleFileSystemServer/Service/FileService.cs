using SimpleFileSystemServer.Model;
using System.Text.RegularExpressions;

namespace SimpleFileSystemServer.Service;

public class FileService {
    /// <summary>
    /// 获取目录下的文件信息
    /// </summary>
    /// <param name="dir">相对路径，以'/'开头</param>
    /// <returns></returns>
    public IList<FileVO> ListFiles(string dir) {
        var directoryInfo = new DirectoryInfo(Path.Combine(Global.WorkingDirectory, dir[1..]));
        // 获取目录
        var dirs = directoryInfo
                .GetDirectories()
                .Select(f => new FileVO(f.Name, dir, true))
                .ToList();
        // 获取文件
        var files = directoryInfo
                .GetFiles()
                .Select(f => new FileVO(f.Name, dir, false))
                .ToList();
        dirs.AddRange(files);
        return dirs;
    }

    private static readonly Regex PathSpliter = new(@"[\\/]");

    /// <summary>
    /// 检查路径范围
    /// </summary>
    /// <param name="path">文件相对路径，以'/'开头</param>
    /// <returns></returns>
    public bool CheckPathRange(string path) {
        path = Path.Combine(Global.WorkingDirectory, path[1..]);
        var absPath = Path.GetFullPath(path);
        string rootNoSpliterPath = PathSpliter.Replace(Global.WorkingDirectory, "").ToLowerInvariant();
        string fileNoSpliterPath = PathSpliter.Replace(absPath, "").ToLowerInvariant();
        return fileNoSpliterPath.StartsWith(rootNoSpliterPath);
    }
}
