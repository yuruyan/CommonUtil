using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CommonUtil {
    public partial class App : Application {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public App() {
            ThreadPool.SetMaxThreads(16, 8);
            ThreadPool.SetMinThreads(8, 4);
            new SplashScreen("/Resource/SplashWindow.png").Show(true);
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            new MainWindow().Show();

            #region 全局错误处理
            // UI线程未捕获异常处理事件
            DispatcherUnhandledException += GlobalDispatcherUnhandledException;
            // 非UI线程未捕获异常处理事件(例如自己创建的一个子线程)
            AppDomain.CurrentDomain.UnhandledException += DomainUnhandledException;
            // Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
            #endregion
        }

        private void TaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) {
            Logger.Error(e);
            MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void DomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            if (e.ExceptionObject is Exception exception) {
                Logger.Error(exception);
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GlobalDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            Logger.Error(e.Exception);
            e.Handled = true;
            MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
