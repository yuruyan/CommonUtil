using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SimpleFileSystemServer.Model;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public record FileVO {
    /// <summary>
    /// 文件名，不包括路径
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 父目录全路径
    /// </summary>
    public string ParentPath { get; set; } = string.Empty;
    /// <summary>
    /// 是否是文件夹
    /// </summary>
    public bool IsDir { get; set; }
    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize { get; set; }

    public FileVO() { }

    public FileVO(string name, string parentPath, bool isDir) {
        Name = name;
        ParentPath = parentPath;
        IsDir = isDir;
    }
}
