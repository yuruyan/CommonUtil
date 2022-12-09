using CommonTools.Utils;
using System.Text.RegularExpressions;

namespace SimpleFileSystemServer.Utils;

#if NET7_0_OR_GREATER
public partial class PathUtils {
#elif NET6_0_OR_GREATER
public class PathUtils {
#endif
#if NET7_0_OR_GREATER
    private static readonly Regex PathSpliter = GetPathSpliterRegex();
#elif NET6_0_OR_GREATER
    private static readonly Regex PathSpliter = new(@"[\\/]");
#endif
#if NET7_0_OR_GREATER
    [GeneratedRegex("[\\\\/]")]
    private static partial Regex GetPathSpliterRegex();
#endif
    /// <summary>
    /// 获取绝对路径
    /// </summary>
    /// <param name="path">文件路径，以 '/' 开头</param>
    /// <returns></returns>
    public static string GetAbsolutePath(string path) {
        return Path.Combine(Global.Argument.RootDir, path[1..]);
    }

    /// <summary>
    /// 标准化路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns>失败返回 '/'</returns>
    public static string Normalize(string? path) {
        if (string.IsNullOrWhiteSpace(path)) {
            return "/";
        }
        path = path.Trim().ReplaceBackSlashWithSlash();
        if (!path.StartsWith('/')) {
            path = '/' + path;
        }
        return path;
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
