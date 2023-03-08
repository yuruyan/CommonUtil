using Microsoft.Extensions.Configuration;
using SimpleFileSystemServer;

namespace SimpleFileSystemServerTests.Utils;

internal static class TestUtils {
    /// <summary>
    /// 初始化
    /// </summary>
    public static void Initialize() {
        var cb = new ConfigurationBuilder();
        cb.AddCommandLine(new string[] {
            $"rootDir={Environment.CurrentDirectory}"
        });
        Global.Initialize(cb.Build());
    }
}
