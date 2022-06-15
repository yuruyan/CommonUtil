using CommonUITools.Utils;
using CommonUtil.Core;
using NLog;
using Ookii.Dialogs.Wpf;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommonUtil.View {
    public partial class SimpleFileSystemServerView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static readonly DependencyProperty SharingDirectoryProperty = DependencyProperty.Register("SharingDirectory", typeof(string), typeof(SimpleFileSystemServerView), new PropertyMetadata(""));
        public static readonly DependencyProperty IsServerStartedProperty = DependencyProperty.Register("IsServerStarted", typeof(bool), typeof(SimpleFileSystemServerView), new PropertyMetadata(false));
        public static readonly DependencyProperty ServerPortProperty = DependencyProperty.Register("ServerPort", typeof(int), typeof(SimpleFileSystemServerView), new PropertyMetadata(3000));
        public static readonly DependencyProperty ServerURLProperty = DependencyProperty.Register("ServerURL", typeof(string), typeof(SimpleFileSystemServerView), new PropertyMetadata(""));

        /// <summary>
        /// 分享文件目录
        /// </summary>
        public string SharingDirectory {
            get { return (string)GetValue(SharingDirectoryProperty); }
            set { SetValue(SharingDirectoryProperty, value); }
        }
        private CommonUtil.Core.SimpleFileSystemServer? SimpleFileSystemServer;
        /// <summary>
        /// 服务器是否启动
        /// </summary>
        public bool IsServerStarted {
            get { return (bool)GetValue(IsServerStartedProperty); }
            set { SetValue(IsServerStartedProperty, value); }
        }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort {
            get { return (int)GetValue(ServerPortProperty); }
            set { SetValue(ServerPortProperty, value); }
        }
        /// <summary>
        /// 服务器 URL
        /// </summary>
        public string ServerURL {
            get { return (string)GetValue(ServerURLProperty); }
            set { SetValue(ServerURLProperty, value); }
        }

        public SimpleFileSystemServerView() {
            InitializeComponent();
            SharingDirectory = Directory.GetCurrentDirectory();
            DependencyPropertyDescriptor.FromProperty(IsServerStartedProperty, typeof(SimpleFileSystemServerView)).AddValueChanged(this, ServerStateChangedHandler);
        }

        /// <summary>
        /// 服务器状态变化 Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerStateChangedHandler(object? sender, EventArgs e) {
            if (IsServerStarted) {
                ServerURL = $"http://{NetworkUtils.GetLocalIpAddress()}:{ServerPort}";
                // 复制到剪贴板
                Clipboard.SetDataObject(ServerURL);
                // 监听停止状态
                SimpleFileSystemServer.Stopped += (s, e) => {
                    Dispatcher.Invoke(() => IsServerStarted = false);
                };
            }
        }

        /// <summary>
        /// 选择分享文件目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectSharingDirectoryClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            var dialog = new VistaFolderBrowserDialog {
                Description = "选择分享目录",
                UseDescriptionForTitle = true
            };
            if (dialog.ShowDialog(Application.Current.MainWindow) == true) {
                SharingDirectory = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 点击目录文字，打开所在目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSharingDirectoryMouseUp(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
            if (sender is TextBlock element) {
                UIUtils.OpenFileInDirectoryAsync(element.Text);
            }
        }

        /// <summary>
        /// 切换服务器状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleServerStateClickHandler(object sender, RoutedEventArgs e) {
            e.Handled = true;
            #region 检查是否选择分享目录
            if (string.IsNullOrEmpty(SharingDirectory)) {
                CommonUITools.Widget.MessageBox.Info("未选择分享目录！");
                return;
            }
            #endregion
            // 关闭服务器
            if (IsServerStarted) {
                StopServer();
                return;
            }
            // 开启服务器
            ThrottleUtils.ThrottleAsync(ToggleServerStateClickHandler, async () => {
                bool state = await StartServerAsync();
                IsServerStarted = state;
                // 开启成功
                if (state) {
                    CommonUITools.Widget.MessageBox.Success("服务器启动成功");
                    return;
                }
                StopServer();
                CommonUITools.Widget.MessageBox.Error("服务器启动失败");
            });
        }

        /// <summary>
        /// 主动停止服务器
        /// </summary>
        private void StopServer() {
            IsServerStarted = false;
            SimpleFileSystemServer?.Stop();
        }

        /// <summary>
        /// 异步启动服务器
        /// </summary>
        /// <returns>成功返回 true，失败返回 false</returns>
        private async Task<bool> StartServerAsync() {
            int port = CommonUtils.Try(GetFreePort, 0);
            // 端口不足
            if (port == 0) {
                CommonUITools.Widget.MessageBox.Error("端口不足");
                return false;
            }
            ServerPort = port;
            SimpleFileSystemServer = new(port, SharingDirectory);
            return await SimpleFileSystemServer.StartAsync();
        }

        /// <summary>
        /// 获取未占用端口
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">端口不足，找不到合适端口</exception>
        private int GetFreePort() {
            int port = ServerPort;
            // 获取占用端口
            var inUserPorts = NetworkUtils.GetInUsePorts().ToHashSet();
            for (; port < ServerPort + 7000 && port < 65536; port++) {
                if (!inUserPorts.Contains(port)) {
                    return port;
                }
            }
            throw new Exception("端口不足");
        }
    }
}
