using NLog;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;

namespace CommonUtilStarter;

internal static class Program {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private const string MainProgram = "CommonUtil.exe";
    private static readonly string BootstrapPipeName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.{Random.Shared.Next(10000, 100000)}";

    [STAThread]
    static void Main() {
        ApplicationConfiguration.Initialize();
        // �ȴ� 5s �����û���˳����Զ���ֹ
        Task.Run(() => {
            Thread.Sleep(5000);
            Application.Exit();
        });
        // ����������
        Task.Run(() => {
            try {
                Process.Start(
                    MainProgram,
                    $"bootstrapPipeName={BootstrapPipeName}"
                );
                StartPipeClient();
            } catch (FileNotFoundException) {
                Log.Error($"File {Path.Combine(Environment.ProcessPath!, "../", MainProgram)} not found");
            } catch (Exception error) {
                Log.Error(error);
            }
        });
        Application.Run(new MainForm());
    }

    /// <summary>
    /// �����ͻ���
    /// </summary>
    private static void StartPipeClient() {
        try {
            using NamedPipeClientStream pipeClient = new("localhost", BootstrapPipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None);
            pipeClient.Connect();
            _ = pipeClient.ReadByte();
            Application.Exit();
        } catch {
            Application.Exit();
        }
    }
}