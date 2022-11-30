using System;
using System.IO;
using System.Windows;

namespace CommonUtil.Core.Model;

/// <summary>
/// 下载信息
/// </summary>
public class DownloadTask : DependencyObject {
    public DownloadTask(string url, DirectoryInfo saveDirectory, string name = "") {
        Url = url;
        SaveDirectory = saveDirectory;
        Name = name;
    }

    /// <summary>
    /// 下载文件 url
    /// </summary>
    public string Url { get; }
    /// <summary>
    /// 文件名
    /// </summary>
    public string Name {
        get { return (string)GetValue(NameProperty); }
        set { SetValue(NameProperty, value); }
    }
    /// <summary>
    /// 保存目录
    /// </summary>
    public DirectoryInfo SaveDirectory {
        get { return (DirectoryInfo)GetValue(SaveDirectoryProperty); }
        set { SetValue(SaveDirectoryProperty, value); }
    }
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
    /// 下载速度，byte / s
    /// </summary>
    public double DownloadSpeed {
        get { return (double)GetValue(DownloadSpeedProperty); }
        set { SetValue(DownloadSpeedProperty, value); }
    }
    /// <summary>
    /// 文件总大小 byte
    /// </summary>
    public long TotalSize {
        get { return (long)GetValue(TotalSizeProperty); }
        set { SetValue(TotalSizeProperty, value); }
    }
    /// <summary>
    /// 已下载大小 byte
    /// </summary>
    public long DownloadedSize {
        get { return (long)GetValue(DownloadedSizeProperty); }
        set { SetValue(DownloadedSizeProperty, value); }
    }
    /// <summary>
    /// 是否暂停
    /// </summary>
    public bool IsPaused {
        get { return (bool)GetValue(IsPausedProperty); }
        set { SetValue(IsPausedProperty, value); }
    }
    /// <summary>
    /// 是否结束
    /// </summary>
    public bool IsFinished { get; set; }
    /// <summary>
    /// 进度 [0-100]
    /// </summary>
    public byte Process {
        get { return (byte)GetValue(ProcessProperty); }
        set { SetValue(ProcessProperty, value); }
    }

    public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DownloadTask), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty SaveDirectoryProperty = DependencyProperty.Register("SaveDirectory", typeof(DirectoryInfo), typeof(DownloadTask), new PropertyMetadata());
    public static readonly DependencyProperty ProcessProperty = DependencyProperty.Register("Process", typeof(byte), typeof(DownloadTask), new PropertyMetadata((byte)0));
    public static readonly DependencyProperty DownloadSpeedProperty = DependencyProperty.Register("DownloadSpeed", typeof(double), typeof(DownloadTask), new PropertyMetadata(0.0));
    public static readonly DependencyProperty TotalSizeProperty = DependencyProperty.Register("TotalSize", typeof(long), typeof(DownloadTask), new PropertyMetadata(0L));
    public static readonly DependencyProperty DownloadedSizeProperty = DependencyProperty.Register("DownloadedSize", typeof(long), typeof(DownloadTask), new PropertyMetadata(0L));
    public static readonly DependencyProperty IsPausedProperty = DependencyProperty.Register("IsPaused", typeof(bool), typeof(DownloadTask), new PropertyMetadata(false));
}
