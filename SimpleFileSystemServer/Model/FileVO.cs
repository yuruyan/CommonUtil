using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SimpleFileSystemServer.Model;
[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public record FileVO {
    // { name: "a.json", parentPath: "/a/b/c", isDir: false },
    public string Name { get; init; } = string.Empty;
    public string ParentPath { get; init; } = string.Empty;
    public bool IsDir { get; init; }

    public FileVO() {

    }

    public FileVO(string name, string parentPath, bool isDir) {
        Name = name;
        ParentPath = parentPath;
        IsDir = isDir;
    }
}
