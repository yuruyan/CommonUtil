using CommonUITools.Route;
using CommonUITools.Utils;
using ModernWpf.Controls;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace CommonUtil.View;

public partial class DownloaderView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly RouterService RouterService;
    private readonly IDictionary<string, Type> NavigationDict = new Dictionary<string, Type>() {
        {"1", typeof(DownloadingView) },
        {"2", typeof(DownloadedView) },
    };
    /// <summary>
    /// 下载选择框
    /// </summary>
    private readonly DownloadInfoDialog DownloadInfoDialog = new();

    public DownloaderView() {
        InitializeComponent();
        RouterService = new RouterService(ContentFrame, NavigationDict.Values);
        // 显式初始化
        _ = RouterService.GetInstance(typeof(DownloadingView));
        _ = RouterService.GetInstance(typeof(DownloadedView));
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
        Console.WriteLine(DownloadInfoDialog.URL + "\n" + DownloadInfoDialog.SaveDir);
    }
}
