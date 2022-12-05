using Microsoft.Extensions.Configuration;

namespace CommonUtil.Model;

/// <summary>
/// 命令行参数
/// </summary>
internal record CommandLineArgument {
    /// <summary>
    /// 引导程序匿名管道名称
    /// </summary>
    public string? BootstrapPipeName { get; init; }

    /// <summary>
    /// 解析
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static CommandLineArgument Parse(string[] args) {
        var configRoot = new ConfigurationBuilder().AddCommandLine(args).Build();
        return new() {
            BootstrapPipeName = configRoot["bootstrapPipeName"],
        };
    }
}
