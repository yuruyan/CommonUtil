using CommonUITools.Route;
using CommonUITools.Utils;
using CommonUtil.Model;
using ModernWpf.Controls;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class DownloaderView : System.Windows.Controls.Page {
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
    private readonly DownloadingView DownloadingView;
    private readonly DownloadedView DownloadedView;
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
        // 显式初始化
        DownloadingView = (DownloadingView)RouterService.GetInstance(typeof(DownloadingView));
        DownloadedView = (DownloadedView)RouterService.GetInstance(typeof(DownloadedView));
        DownloadingTaskList = DownloadingView.DownloadTaskList;
        DownloadedTaskList = DownloadedView.DownloadTaskList;
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
            DownloadingView.DownloadTaskList.Remove(e);
            MessageBox.Error($"下载 {e.Name} 失败");
        });
    }

    /// <summary>
    /// 下载成功 Handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DownloadCompletedHandler(object? sender, DownloadTask e) {
        Dispatcher.Invoke(() => {
            DownloadingView.DownloadTaskList.Remove(e);
            MessageBox.Success($"下载 {e.Name} 成功");
            DownloadedView.DownloadTaskList.Add(e);
        });
    }

    /// <summary>
    /// 导航改变
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void NavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
        if (args.SelectedItem is NavigationViewItem viewItem) {
            RouterService.Navigate(
                NavigationDict[CommonUtils.NullCheck(viewItem.Tag.ToString())]
            );
        }
    }

    /// <summary>
    /// 下载按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DownloadTaskMouseUpHandler(object sender, MouseButtonEventArgs e) {
        if (await DownloadInfoDialog.ShowAsync() != ContentDialogResult.Primary) {
            return;
        }
        var urls = DownloadInfoDialog.URL.Split('\n').Where(s => s.Trim().Any());
        foreach (var url in urls) {
            DownloadingView.DownloadTaskList.Add(
                Downloader.Download(url, new(DownloadInfoDialog.SaveDir))
            );
        }
        MessageBox.Info($"开始下载");
    }
}
