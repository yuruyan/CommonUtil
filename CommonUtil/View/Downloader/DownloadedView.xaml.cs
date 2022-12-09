namespace CommonUtil.View;

public partial class DownloadedView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty DownloadTaskListProperty = DependencyProperty.Register("DownloadTaskList", typeof(ObservableCollection<DownloadTask>), typeof(DownloadedView), new PropertyMetadata());

    /// <summary>
    /// 下载任务列表（View）
    /// </summary>
    public ObservableCollection<DownloadTask> DownloadTaskList {
        get { return (ObservableCollection<DownloadTask>)GetValue(DownloadTaskListProperty); }
        private set { SetValue(DownloadTaskListProperty, value); }
    }

    public DownloadedView() {
        DownloadTaskList = new();
        InitializeComponent();
    }

    /// <summary>
    /// 打开所在文件夹
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenFolderClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is DownloadTask task) {
            string filepath = Path.Combine(task.SaveDirectory.FullName, task.FileName);
            // 文件存在
            if (File.Exists(filepath)) {
                UIUtils.OpenFileInExplorerAsync(filepath);
            } else {
                // 不存在则打开所在文件夹
                UIUtils.OpenFileInExplorerAsync(task.SaveDirectory.FullName);
            }
        }
    }

    /// <summary>
    /// 移除下载文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RemoveHistoryClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        DownloadTaskListBox.SelectedItems
            .Cast<DownloadTask>()
            .ToList()
            .ForEach(task => {
                DownloadTaskList.Remove(task);
            });
    }

    /// <summary>
    /// 打开文件点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenFileClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is DownloadTask result) {
            UIUtils.OpenFileWithAsync(Path.Join(result.SaveDirectory.FullName, result.FileName));
        }
    }

    /// <summary>
    /// 复制下载链接
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyLinkClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is DownloadTask result) {
            Clipboard.SetDataObject(result.Url);
            MessageBox.Success("已复制");
        }
    }
}
