namespace CommonUtil.View;

public partial class FileMergeView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 更新进度间隔时间
    /// </summary>
    private const int UpdateProcessInterval = 250;
    public static readonly DependencyProperty MergeFileDirectoryProperty = DependencyProperty.Register("MergeFileDirectory", typeof(string), typeof(FileMergeView), new PropertyMetadata(""));
    public static readonly DependencyProperty MergeFileSavePathProperty = DependencyProperty.Register("MergeFileSavePath", typeof(string), typeof(FileMergeView), new PropertyMetadata(""));
    public static readonly DependencyProperty MergeFilesProperty = DependencyProperty.Register("MergeFiles", typeof(ObservableCollection<string>), typeof(FileMergeView), new PropertyMetadata());
    public static readonly DependencyProperty TotalFileSizeProperty = DependencyProperty.Register("TotalFileSize", typeof(ulong), typeof(FileMergeView), new PropertyMetadata(0UL));
    public static readonly DependencyProperty WorkingProcessProperty = DependencyProperty.Register("WorkingProcess", typeof(double), typeof(FileMergeView), new PropertyMetadata(0.0));
    public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register("IsWorking", typeof(bool), typeof(FileMergeView), new PropertyMetadata(false));

    /// <summary>
    /// 合并文件输入
    /// </summary>
    public string MergeFileDirectory {
        get { return (string)GetValue(MergeFileDirectoryProperty); }
        set { SetValue(MergeFileDirectoryProperty, value); }
    }
    /// <summary>
    /// 合并文件保存路径
    /// </summary>
    public string MergeFileSavePath {
        get { return (string)GetValue(MergeFileSavePathProperty); }
        set { SetValue(MergeFileSavePathProperty, value); }
    }
    /// <summary>
    /// 合并文件路径列表，更新只进行替换
    /// </summary>
    public ObservableCollection<string> MergeFiles {
        get { return (ObservableCollection<string>)GetValue(MergeFilesProperty); }
        set { SetValue(MergeFilesProperty, value); }
    }
    /// <summary>
    /// 文件合并总大小
    /// </summary>
    public ulong TotalFileSize {
        get { return (ulong)GetValue(TotalFileSizeProperty); }
        set { SetValue(TotalFileSizeProperty, value); }
    }
    /// <summary>
    /// 合并文件进度
    /// </summary>
    public double WorkingProcess {
        get { return (double)GetValue(WorkingProcessProperty); }
        set { SetValue(WorkingProcessProperty, value); }
    }
    /// <summary>
    /// 是否正在合并文件
    /// </summary>
    public bool IsWorking {
        get { return (bool)GetValue(IsWorkingProperty); }
        set { SetValue(IsWorkingProperty, value); }
    }
    /// <summary>
    /// 上次合并文件更新时间
    /// </summary>
    private DateTime LastMergeFileUpdateTime = DateTime.Now;
    /// <summary>
    /// 正在合并的文件路径
    /// </summary>
    private string WorkingMergeFilePath = string.Empty;
    /// <summary>
    /// 是否请求取消
    /// </summary>
    private bool IsCancelRequested = false;
    /// <summary>
    /// 当前 Window
    /// </summary>
    private Window CurrentWindow = Application.Current.MainWindow;

    public FileMergeView() {
        DependencyPropertyDescriptor.FromProperty(MergeFilesProperty, typeof(FileMergeView))
            .AddValueChanged(this, CalculateTotalFileSizeHandler);
        MergeFiles = new();
        InitializeComponent();
        UIUtils.SetLoadedOnceEventHandler(this, (_, _) => CurrentWindow = Window.GetWindow(this));
    }

    /// <summary>
    /// 计算文件总大小
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CalculateTotalFileSizeHandler(object? sender, EventArgs e) {
        TotalFileSize = (ulong)MergeFiles.Select(f => new FileInfo(f).Length).Sum();
    }

    /// <summary>
    /// 检查合并文件输入有效性
    /// </summary>
    /// <returns></returns>
    private bool CheckInputValidation() {
        if (!UIUtils.CheckInputNullOrEmpty(new KeyValuePair<string?, string>[] {
            new (MergeFileSavePath, "文件保存路径不能为空"),
            new (MergeFileDirectory, "合并文件目录不能为空"),
            new (MergeFiles.Any() ? "." : null, "合并文件列表不能为空"),
        })) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 选择合并文件保存路径
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectMergeFileSaveClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var dialog = new SaveFileDialog {
            Filter = "All Files|*.*"
        };
        if (dialog.ShowDialog() != true) {
            return;
        }
        MergeFileSavePath = dialog.FileName;
    }

    /// <summary>
    /// 选择合并文件目录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectMergeFileDirClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var dialog = new VistaFolderBrowserDialog {
            Description = "选择合并文件夹",
            UseDescriptionForTitle = true
        };
        if (dialog.ShowDialog(CurrentWindow) != true) {
            return;
        }
        MergeFileDirectory = dialog.SelectedPath;
        // 读取文件列表
        string[] files = Directory.GetFiles(MergeFileDirectory);
        Array.Sort(files);
        MergeFiles = new(files);
    }

    /// <summary>
    /// 合并文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MergeFileClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (IsWorking) {
            return;
        }
        if (!CheckInputValidation()) {
            return;
        }

        var files = MergeFiles;
        var savePath = MergeFileSavePath;
        WorkingMergeFilePath = MergeFileSavePath;
        IsWorking = true;
        WorkingProcess = 0;
        // 开始合并
        try {
            await Task.Run(() => FileMergeSplit.MergeFile(
                files,
                savePath,
                process => {
                    if ((DateTime.Now - LastMergeFileUpdateTime).TotalMilliseconds > UpdateProcessInterval) {
                        LastMergeFileUpdateTime = DateTime.Now;
                        Dispatcher.Invoke(() => WorkingProcess = process);
                    }
                }
            ));
            // 没有取消则提示
            if (!IsCancelRequested) {
                MessageBoxUtils.Success("合并完成");
            }
        } catch (Exception error) {
            MessageBoxUtils.Error($"合并文件失败：{error.Message}");
        }
        IsCancelRequested = IsWorking = false;
    }

    /// <summary>
    /// 取消任务
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelClickHandler(object sender, RoutedEventArgs e) {
        IsCancelRequested = true;
        FileMergeSplit.CancelMergeFile(WorkingMergeFilePath);
    }
}
