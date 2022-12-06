namespace CommonUtil.View;

public partial class DownloadTaskInfoWidget : UserControl {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(DownloadTaskInfoWidget), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty TransferSizeProperty = DependencyProperty.Register("TransferSize", typeof(long), typeof(DownloadTaskInfoWidget), new PropertyMetadata(0L));
    public static readonly DependencyProperty TotalSizeProperty = DependencyProperty.Register("TotalSize", typeof(long), typeof(DownloadTaskInfoWidget), new PropertyMetadata(0L));

    public string FilePath {
        get { return (string)GetValue(FilePathProperty); }
        set { SetValue(FilePathProperty, value); }
    }
    /// <summary>
    /// 下载/上传大小，为 0 则隐藏
    /// </summary>
    public long TransferSize {
        get { return (long)GetValue(TransferSizeProperty); }
        set { SetValue(TransferSizeProperty, value); }
    }
    public long TotalSize {
        get { return (long)GetValue(TotalSizeProperty); }
        set { SetValue(TotalSizeProperty, value); }
    }

    public DownloadTaskInfoWidget() {
        InitializeComponent();
    }
}
