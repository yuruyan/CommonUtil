namespace SimpleFileSystemServer;

public class Global {
    public static string WorkingDirectory { get; private set; } = default!;

    public Global(IConfiguration configuration) {
        WorkingDirectory ??= configuration["dir"];
    }
}
