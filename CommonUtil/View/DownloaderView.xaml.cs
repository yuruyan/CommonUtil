using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class DownloaderView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public DownloaderView() {
        InitializeComponent();
    }

}
