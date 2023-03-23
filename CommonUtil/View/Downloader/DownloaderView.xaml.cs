using System.Net;

namespace CommonUtil.View;

public partial class DownloaderView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty DownloadingTaskListProperty = DependencyProperty.Register("DownloadingTaskList", typeof(ObservableCollection<DownloadTask>), typeof(DownloaderView), new PropertyMetadata());
    public static readonly DependencyProperty DownloadedTaskListProperty = DependencyProperty.Register("DownloadedTaskList", typeof(ObservableCollection<DownloadTask>), typeof(DownloaderView), new PropertyMetadata());

    private readonly RouterService RouterService;
    private readonly IDictionary<string, Type> NavigationDict = new Dictionary<string, Type>() {
        {"1", typeof(DownloadingView) },
        {"2", typeof(DownloadedView) },
    };
    /// <summary>
    /// 下载选择框
    /// </summary>
    private readonly DownloadInfoDialog DownloadInfoDialog = new();
    //private readonly DownloadingView DownloadingViewInstance;
    //private readonly DownloadedView DownloadedViewInstance;
    private readonly Core.Downloader Downloader = new();
    /// <summary>
    /// 下载任务列表，引用 DownloadingView
    /// </summary>
    public ObservableCollection<DownloadTask> DownloadingTaskList {
        get { return (ObservableCollection<DownloadTask>)GetValue(DownloadingTaskListProperty); }
        private set { SetValue(DownloadingTaskListProperty, value); }
    }
    /// <summary>
    /// 下载任务列表，引用 DownloadedView
    /// </summary>
    public ObservableCollection<DownloadTask> DownloadedTaskList {
        get { return (ObservableCollection<DownloadTask>)GetValue(DownloadedTaskListProperty); }
        private set { SetValue(DownloadedTaskListProperty, value); }
    }

    public DownloaderView() {
        InitializeComponent();
        RouterService = new RouterService(ContentFrame, NavigationDict.Values);
        NavigationUtils.EnableNavigation(
            NavigationView,
            RouterService,
            ContentFrame
        );
        // 显式初始化
        DownloadingTaskList = ((DownloadingView)RouterService.GetInstance(typeof(DownloadingView))).DownloadTaskList;
        DownloadedTaskList = ((DownloadedView)RouterService.GetInstance(typeof(DownloadedView))).DownloadTaskList;
        Downloader.DownloadCompleted += DownloadCompletedHandler;
        Downloader.DownloadFailed += DownloadFailedHandler;
    }

    /// <summary>
    /// 下载失败 Handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DownloadFailedHandler(object? sender, DownloadTask e) {
        Dispatcher.Invoke(() => {
            DownloadingTaskList.Remove(e);
            DownloadedTaskList.Add(e);
        });
    }

    /// <summary>
    /// 下载成功 Handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DownloadCompletedHandler(object? sender, DownloadTask e) {
        Dispatcher.Invoke(() => {
            DownloadingTaskList.Remove(e);
            DownloadedTaskList.Add(e);
        });
    }

    /// <summary>
    /// 点击下载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DownloadTaskClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (await DownloadInfoDialog.ShowAsync() != ModernWpf.Controls.ContentDialogResult.Primary) {
            return;
        }
        bool anySuccess = false;
        WebProxy? proxy = null;
        var urls = DownloadInfoDialog.URL.ReplaceLineFeedWithLinuxStyle()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(u => (u.StartsWith("http://") || u.StartsWith("https://")) ? u : $"https://{u}");
        // 设置代理
        if (DownloadInfoDialog.HasProxy) {
            proxy = new(DownloadInfoDialog.FullProxyAddress, true);
        }
        foreach (var url in urls) {
            var task = Downloader.Download(
                url,
                new(DownloadInfoDialog.SaveDir),
                proxy
            );
            if (task is null) {
                continue;
            }
            anySuccess = true;
            DownloadingTaskList.Add(task);
        }
        // 如果任意一个任务开始
        if (anySuccess) {
            MessageBoxUtils.Info($"开始下载");
        }
    }
}
