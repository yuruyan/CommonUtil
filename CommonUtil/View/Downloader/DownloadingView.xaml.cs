using CommonUtil.Core.Model;

namespace CommonUtil.View;
public partial class DownloadingView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty DownloadTaskListProperty = DependencyProperty.Register("DownloadTaskList", typeof(ObservableCollection<DownloadTask>), typeof(DownloadingView), new PropertyMetadata());

    /// <summary>
    /// 下载任务列表（View）
    /// </summary>
    public ObservableCollection<DownloadTask> DownloadTaskList {
        get { return (ObservableCollection<DownloadTask>)GetValue(DownloadTaskListProperty); }
        private set { SetValue(DownloadTaskListProperty, value); }
    }

    public DownloadingView() {
        DownloadTaskList = new();
        InitializeComponent();
    }
}
