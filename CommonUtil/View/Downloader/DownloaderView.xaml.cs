using CommonUITools.Route;
using CommonUITools.Utils;
using CommonUtil.Core;
using Microsoft.Win32;
using ModernWpf.Controls;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommonUtil.View;

public partial class DownloaderView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly RouterService RouterService;

    private readonly IDictionary<string, Type> NavigationDict = new Dictionary<string, Type>() {
        {"1", typeof(DownloadingView) },
        {"2", typeof(DownloadedView) },
    };

    public DownloaderView() {
        InitializeComponent();
        RouterService = new RouterService(ContentFrame, NavigationDict.Values);
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

    private void DownloadTaskMouseUpHandler(object sender, MouseButtonEventArgs e) {
        Logger.Debug("start download task");
    }
}
