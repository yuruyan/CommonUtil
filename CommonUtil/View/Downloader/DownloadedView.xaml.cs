using CommonUtil.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
}
