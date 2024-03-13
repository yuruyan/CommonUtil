using System.IO.Pipes;
using System.Reflection;

namespace CommonUtil;

public partial class App : Application {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly string? BootstrapPipeName = Global.CommandLineArgument.BootstrapPipeName;

    public App() {
        // 确保路径为当前可执行程序目录
        Environment.CurrentDirectory = Path.GetDirectoryName(Environment.ProcessPath)!;
        if (BootstrapPipeName is null) {
            new SplashScreen(
                Assembly.Load("CommonUtil.Data"),
                "Resources/SplashWindow.png"
            ).Show(true);
        }
    }

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        var mainWindow = new MainWindow();
        // 显式初始化 FileIcon
        mainWindow.SetLoadedOnceEventHandler((_, _) => {
            Task.Run(() => FileIconUtils.InitializeExplicitly());
            if (BootstrapPipeName is null) {
                return;
            }
            Task.Run(() => {
                TaskUtils.Try(() => {
                    using NamedPipeServerStream pipeServer = new(BootstrapPipeName, PipeDirection.InOut, 1);
                    Logger.Debug("Wait for connection");
                    pipeServer.WaitForConnection();
                    pipeServer.Write(Encoding.UTF8.GetBytes("close"));
                    pipeServer.Flush();
                    Logger.Debug("Close pipe");
                });
            });
        });
        mainWindow.Show();
        #region 全局错误处理
        // UI线程未捕获异常处理事件
        Current.DispatcherUnhandledException += GlobalDispatcherUnhandledException;
        // 非UI线程未捕获异常处理事件(例如自己创建的一个子线程)
        AppDomain.CurrentDomain.UnhandledException += DomainUnhandledException;
        // Task线程内未捕获异常处理事件
        TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
        #endregion
    }

    private void TaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) {
        e.SetObserved();
        if (e.Exception is not null) {
            Logger.Error(e.Exception, "TaskSchedulerUnobservedTaskException");
            //// 提示信息
            //System.Windows.MessageBox.Show(
            //    e.Exception?.Message,
            //    "TaskSchedulerUnobservedTaskException",
            //    MessageBoxButton.OK,
            //    MessageBoxImage.Error
            //);
        }
    }

    private void DomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
        if (e.ExceptionObject is Exception exception) {
            Logger.Fatal(exception, "DomainUnhandledException");
            // 提示信息
            System.Windows.MessageBox.Show(
                exception.Message,
                "DomainUnhandledException",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
        Environment.Exit(-1);
    }

    private void GlobalDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        // 提示信息
        if (e.Exception is not null) {
            Logger.Fatal(e.Exception, "GlobalDispatcherUnhandledException");
            System.Windows.MessageBox.Show(
                Current.MainWindow,
                e.Exception.Message,
                "GlobalDispatcherUnhandledException",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
        Environment.Exit(-1);
    }
}
