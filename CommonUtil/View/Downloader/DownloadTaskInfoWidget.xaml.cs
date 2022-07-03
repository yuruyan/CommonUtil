using NLog;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class DownloadTaskInfoWidget : UserControl {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(DownloadTaskInfoWidget), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty TransferSizeProperty = DependencyProperty.Register("TransferSize", typeof(ulong), typeof(DownloadTaskInfoWidget), new PropertyMetadata(0UL));
    public static readonly DependencyProperty TotalSizeProperty = DependencyProperty.Register("TotalSize", typeof(ulong), typeof(DownloadTaskInfoWidget), new PropertyMetadata(0UL));

    public string FilePath {
        get { return (string)GetValue(FilePathProperty); }
        set { SetValue(FilePathProperty, value); }
    }
    /// <summary>
    /// 下载/上传大小，为 0 则隐藏
    /// </summary>
    public ulong TransferSize {
        get { return (ulong)GetValue(TransferSizeProperty); }
        set { SetValue(TransferSizeProperty, value); }
    }
    public ulong TotalSize {
        get { return (ulong)GetValue(TotalSizeProperty); }
        set { SetValue(TotalSizeProperty, value); }
    }

    public DownloadTaskInfoWidget() {
        InitializeComponent();
    }
}
