using System.Text.RegularExpressions;

namespace SimpleFileSystemServer.Utils;

public partial class PathUtils {
    private static readonly Regex PathSpliter = GetPathSpliterRegex();

    [GeneratedRegex("[\\\\/]")]
    private static partial Regex GetPathSpliterRegex();

    /// <summary>
    /// 获取绝对路径
    /// </summary>
    /// <param name="path">文件路径，以 '/' 开头</param>
    /// <returns></returns>
    public static string GetAbsolutePath(string path) {
        return Path.Combine(Global.Argument.RootDir, path[1..]);
    }

    /// <summary>
    /// 检查路径范围是否有效
    /// </summary>
    /// <param name="path">文件绝对路径，以'/'开头</param>
    /// <returns></returns>
    public static bool CheckPathRange(string path) {
        var absPath = GetAbsolutePath(path);
        string rootNoSpliterPath = PathSpliter.Replace(Global.Argument.RootDir, "").ToLowerInvariant();
        string fileNoSpliterPath = PathSpliter.Replace(absPath, "").ToLowerInvariant();
        return fileNoSpliterPath.StartsWith(rootNoSpliterPath);
    }
}
