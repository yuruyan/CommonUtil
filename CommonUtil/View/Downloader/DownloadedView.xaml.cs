using CommonUtil.Core.Model;

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
            string filepath = Path.Combine(task.SaveDirectory.FullName, task.Name);
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
    /// 删除下载文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteHistoryClickHandler(object sender, RoutedEventArgs e) {

    }
}
