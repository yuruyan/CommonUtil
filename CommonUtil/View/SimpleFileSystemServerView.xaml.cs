using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CommonUtil.View;

public partial class SimpleFileSystemServerView : Page {
    //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// 端口被占用时下一个端口即为当前端口值+该值
    /// </summary>
    private const int PortInterval = 256;

    public static readonly DependencyProperty SharingDirectoryProperty = DependencyProperty.Register("SharingDirectory", typeof(string), typeof(SimpleFileSystemServerView), new PropertyMetadata(""));
    public static readonly DependencyProperty IsServerStartedProperty = DependencyProperty.Register("IsServerStarted", typeof(bool), typeof(SimpleFileSystemServerView), new PropertyMetadata(false, IsServerStartedPropertyChangedHandler));
    public static readonly DependencyProperty ServerPortProperty = DependencyProperty.Register("ServerPort", typeof(int), typeof(SimpleFileSystemServerView), new PropertyMetadata(3000));
    public static readonly DependencyProperty ServerURLProperty = DependencyProperty.Register("ServerURL", typeof(string), typeof(SimpleFileSystemServerView), new PropertyMetadata(""));
    public static readonly DependencyProperty IPAddressesProperty = DependencyProperty.Register("IPAddresses", typeof(ObservableCollection<string>), typeof(SimpleFileSystemServerView), new PropertyMetadata());

    /// <summary>
    /// 分享文件目录
    /// </summary>
    public string SharingDirectory {
        get { return (string)GetValue(SharingDirectoryProperty); }
        set { SetValue(SharingDirectoryProperty, value); }
    }
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
    /// <summary>
    /// IP 地址
    /// </summary>
    public ObservableCollection<string> IPAddresses {
        get { return (ObservableCollection<string>)GetValue(IPAddressesProperty); }
        set { SetValue(IPAddressesProperty, value); }
    }

    private SimpleFileSystemServer? SimpleFileSystemServer;
    /// <summary>
    /// 当前 Window
    /// </summary>
    private Window CurrentWindow = Application.Current.MainWindow;
    private readonly VistaFolderBrowserDialog SelectSharingFolderDialog = new() {
        Description = "选择分享目录",
        UseDescriptionForTitle = true
    };

    public SimpleFileSystemServerView() {
        IPAddresses = new();
        InitializeComponent();
        SharingDirectory = Directory.GetCurrentDirectory();
        this.SetLoadedOnceEventHandler(static (sender, _) => {
            if (sender is SimpleFileSystemServerView self) {
                self.CurrentWindow = Window.GetWindow(self);
            }
        });
    }

    private static void IsServerStartedPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not SimpleFileSystemServerView self) {
            return;
        }
        if (e.NewValue is true) {
            //self.ServerURL = $"http://{NetworkUtils.GetLocalIpAddress()}:{self.ServerPort}";
            var ipAddresses = TaskUtils.Try(() => self.GetIPAddresses());
            if (ipAddresses is null) {
                MessageBoxUtils.Error("获取 IP 失败");
            } else {
                ipAddresses.Sort();
                self.IPAddresses = new(ipAddresses.Select(ip => $"http://{ip}:{self.ServerPort}"));
            }
            // 监听停止状态
            self.SimpleFileSystemServer!.Stopped += self.ServerStoppedEventHandler;
        }
    }

    private void ServerStoppedEventHandler(object? o, EventArgs args) {
        IsServerStarted = false;
        SimpleFileSystemServer!.Stopped -= ServerStoppedEventHandler;
    }

    /// <summary>
    /// 选择分享文件目录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectSharingDirectoryClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (SelectSharingFolderDialog.ShowDialog(CurrentWindow) == true) {
            SharingDirectory = SelectSharingFolderDialog.SelectedPath;
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
            element.Text.OpenFileInExplorerAsync();
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
            MessageBoxUtils.Info("未选择分享目录！");
            return;
        }
        #endregion
        // 关闭服务器
        if (IsServerStarted) {
            StopServer();
            return;
        }
        // 开启服务器
        ThrottleUtils.ThrottleAsync($"{nameof(SimpleFileSystemServerView)}  |  {nameof(ToggleServerStateClickHandler)}|{GetHashCode()}", async () => {
            bool state = await StartServerAsync();
            IsServerStarted = state;
            // 开启成功
            if (state) {
                MessageBoxUtils.Success("服务器启动成功");
                return;
            }
            StopServer();
            MessageBoxUtils.Error("服务器启动失败");
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
        int port = TaskUtils.Try(GetFreePort, 0);
        // 端口不足
        if (port == 0) {
            MessageBoxUtils.Error("端口不足");
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
        var inUsePorts = NetworkUtils.GetInUsePorts().ToHashSet();
        for (; port < ServerPort + 7000 && port < 65536; port += PortInterval) {
            if (!inUsePorts.Contains(port) && !CheckPortInUse(port)) {
                return port;
            }
        }
        throw new Exception("端口不足");
    }

    /// <summary>
    /// 检查指定端口是否被占用
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    private bool CheckPortInUse(int port) {
        try {
            var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            tcpListener.Start();
            tcpListener.Stop();
            return false;
        } catch {
            return true;
        }
    }

    /// <summary>
    /// 获取 IP 地址
    /// </summary>
    /// <returns></returns>
    private List<string> GetIPAddresses() {
        var ips = new List<string> {
            Environment.MachineName
        };
        foreach (var item in NetworkInterface.GetAllNetworkInterfaces()) {
            var addressInfo = item
                .GetIPProperties()
                .UnicastAddresses
                .FirstOrDefault(info => info.Address.AddressFamily == AddressFamily.InterNetwork);
            if (addressInfo is not null) {
                if (addressInfo.Address.ToString() is var ip && !ip.StartsWith("169.")) {
                    ips.Add(ip);
                }
            }
        }
        return ips;
    }

    /// <summary>
    /// 复制链接
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyIPAddressClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender.GetElementDataContext<string>() is string ip) {
            Clipboard.SetDataObject(ip);
            MessageBoxUtils.Success("已复制");
        }
    }
}
