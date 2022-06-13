namespace SimpleFileSystemServer;

public class Global {
    public static string WorkingDirectory { get; private set; }

    public Global(IConfiguration configuration) {
        if (WorkingDirectory == null) {
            WorkingDirectory = configuration["dir"];
        }
    }
}
