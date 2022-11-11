using NLog;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CommonUtil;

public partial class App : Application {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public App() {
        new SplashScreen("/Resource/SplashWindow.png").Show(true);
    }

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        new MainWindow().Show();

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
        Logger.Error(e);
        // 提示信息
        MessageBox.Show(
            Current.MainWindow,
            e.Exception.Message,
            "错误",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }

    private void DomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
        if (e.ExceptionObject is Exception exception) {
            Logger.Fatal(exception);
            // 提示信息
            MessageBox.Show(
                Current.MainWindow,
                exception.Message,
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            Shutdown();
        }
    }

    private void GlobalDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        e.Handled = true;
        Logger.Fatal(e.Exception);
        string message = e.Exception switch {
            FileNotFoundException f => $"文件{f.FileName}找不到",
            DirectoryNotFoundException d => $"目录找不到\n{d.Message}",
            _ => e.Exception.Message
        };
        // 提示信息
        MessageBox.Show(
            Current.MainWindow,
            message,
            "错误",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }
}
