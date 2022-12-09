using System.Net;

namespace CommonUtil.Core.Model;

/// <summary>
/// 下载信息
/// </summary>
public class DownloadTask : FileProcessStatus {
    public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DownloadTask), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty DownloadSpeedProperty = DependencyProperty.Register("DownloadSpeed", typeof(double), typeof(DownloadTask), new PropertyMetadata(0.0));
    public static readonly DependencyProperty DownloadedSizeProperty = DependencyProperty.Register("DownloadedSize", typeof(long), typeof(DownloadTask), new PropertyMetadata(0L));

    public DownloadTask(string url, DirectoryInfo saveDirectory, string filename = "") {
        Url = url;
        SaveDirectory = saveDirectory;
        FileName = filename;
    }

    /// <summary>
    /// 下载速度，byte / s
    /// </summary>
    public double DownloadSpeed {
        get { return (double)GetValue(DownloadSpeedProperty); }
        set { SetValue(DownloadSpeedProperty, value); }
    }
    /// <summary>
    /// 已下载大小 byte
    /// </summary>
    public long DownloadedSize {
        get { return (long)GetValue(DownloadedSizeProperty); }
        set { SetValue(DownloadedSizeProperty, value); }
    }
    /// <summary>
    /// 下载文件 url
    /// </summary>
    public string Url { get; }
    /// <summary>
    /// 保存目录
    /// </summary>
    public DirectoryInfo SaveDirectory { get; }
    /// <summary>
    /// 上次更新时间
    /// </summary>
    public DateTime LastUpdateTime { get; set; } = DateTime.Now;
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime BeginTime { get; } = DateTime.Now;
    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime FinishTime { get; set; } = DateTime.Now;
    /// <summary>
    /// 代理
    /// </summary>
    public WebProxy? Proxy { get; set; }
}
