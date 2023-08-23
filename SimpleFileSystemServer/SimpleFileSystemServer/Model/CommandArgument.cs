namespace SimpleFileSystemServer.Model;

/// <summary>
/// 命令行参数
/// </summary>
public record CommandArgument {
    public string RootDir { get; init; }
    public EnvironmentMode EnvironmentMode { get; init; } = EnvironmentMode.Production;

    public CommandArgument(string rootDir) {
        RootDir = Path.GetFullPath(rootDir);
    }
}
