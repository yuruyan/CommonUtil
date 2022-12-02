using CommonTools.Utils;

namespace SimpleFileSystemServer;

public static class Global {
    /// <summary>
    /// 命令行参数
    /// </summary>
    public static CommandArgument Argument { get; private set; } = default!;

    /// <summary>
    /// 初始化，多次调用无效
    /// </summary>
    /// <param name="configuration"></param>
    public static void Initialize(IConfiguration configuration) {
        TaskUtils.EnsureCalledOnce(Initialize, () => {
            Argument = ParseCommandLine(configuration);
        });
    }

    /// <summary>
    /// 解析命令行参数
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">解析失败</exception>
    private static CommandArgument ParseCommandLine(IConfiguration configuration) {
        if (configuration["rootDir"] is not string rootDir || string.IsNullOrWhiteSpace(rootDir)) {
            throw new NullReferenceException("rootDir cannot be null or empty");
        }
        return new(rootDir);
    }
}
