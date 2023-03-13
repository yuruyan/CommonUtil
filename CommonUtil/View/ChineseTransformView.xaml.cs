namespace CommonUtil.View;

public partial class ChineseTransformView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(ChineseTransformView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(ChineseTransformView), new PropertyMetadata(""));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ChineseTransformView), new PropertyMetadata(true));
    public static readonly DependencyProperty IsToSimplifiedWorkingProperty = DependencyProperty.Register("IsToSimplifiedWorking", typeof(bool), typeof(ChineseTransformView), new PropertyMetadata(false));
    public static readonly DependencyProperty IsToTraditionalWorkingProperty = DependencyProperty.Register("IsToTraditionalWorking", typeof(bool), typeof(ChineseTransformView), new PropertyMetadata(false));
    public static readonly DependencyProperty FileProcessStatusesProperty = DependencyProperty.Register("FileProcessStatuses", typeof(ObservableCollection<FileProcessStatus>), typeof(ChineseTransformView), new PropertyMetadata());

    /// <summary>
    /// 保存文件对话框
    /// </summary>
    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "文本文件|*.txt|All Files|*.*"
    };
    /// <summary>
    /// 保存目录对话框
    /// </summary>
    private readonly VistaFolderBrowserDialog SaveDirectoryDialog = new() {
        Description = "选择保存目录",
        UseDescriptionForTitle = true
    };

    /// <summary>
    /// 输入文本
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }
    /// <summary>
    /// 输出文本
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }
    /// <summary>
    /// 是否扩宽
    /// </summary>
    public bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }
    /// <summary>
    /// 是否正在转简体
    /// </summary>
    public bool IsToSimplifiedWorking {
        get { return (bool)GetValue(IsToSimplifiedWorkingProperty); }
        set { SetValue(IsToSimplifiedWorkingProperty, value); }
    }
    /// <summary>
    /// 是否正在转繁体
    /// </summary>
    public bool IsToTraditionalWorking {
        get { return (bool)GetValue(IsToTraditionalWorkingProperty); }
        set { SetValue(IsToTraditionalWorkingProperty, value); }
    }
    /// <summary>
    /// 文件处理列表
    /// </summary>
    public ObservableCollection<FileProcessStatus> FileProcessStatuses {
        get { return (ObservableCollection<FileProcessStatus>)GetValue(FileProcessStatusesProperty); }
        set { SetValue(FileProcessStatusesProperty, value); }
    }
    /// <summary>
    /// 当前 Window
    /// </summary>
    private Window Window = App.Current.MainWindow;
    private CancellationTokenSource ToSimplifiedCancellationTokenSource = new();
    private CancellationTokenSource ToTraditionalCancellationTokenSource = new();

    public ChineseTransformView() {
        FileProcessStatuses = new();
        InitializeComponent();
        // 后台加载
        Task.Run(() => ChineseTransform.InitializeExplicitly());
        // 响应式布局
        UIUtils.SetLoadedOnceEventHandler(this, (_, _) => {
            Window = Window.GetWindow(this);
            double expansionThreshold = (double)Resources["ExpansionThreshold"];
            IsExpanded = Window.ActualWidth >= expansionThreshold;
            DependencyPropertyDescriptor
                .FromProperty(Window.ActualWidthProperty, typeof(Window))
                .AddValueChanged(Window, (_, _) => {
                    IsExpanded = Window.ActualWidth >= expansionThreshold;
                });
        });
    }

    /// <summary>
    /// 文字转简体
    /// </summary>
    private void StringToSimplified() {
        OutputText = ChineseTransform.ToSimplified(InputText);
    }

    /// <summary>
    /// 文件转繁体
    /// </summary>
    private void StringToTraditional() {
        OutputText = ChineseTransform.ToTraditional(InputText);
    }

    /// <summary>
    /// 文件转简体
    /// </summary>
    [NoException]
    private async Task FileToSimplified() {
        var fileNames = DragDropTextBox.FileNames;
        if (fileNames.Count == 1) {
            await FileToSimplified(fileNames[0]);
        } else {
            await MultiFilesToSimplified(fileNames);
        }
    }

    /// <summary>
    /// 文件转繁体
    /// </summary>
    [NoException]
    private async Task FileToTraditional() {
        var fileNames = DragDropTextBox.FileNames;
        if (fileNames.Count == 1) {
            await FileToTraditional(fileNames[0]);
        } else {
            await MultiFilesToTraditional(fileNames);
        }
    }

    /// <summary>
    /// 转换一个文件为简体
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    [NoException]
    private async Task FileToSimplified(string filename) {
        await FileProcessUtils.ProcessOneFileAsync(
            filename,
            SaveFileDialog,
            ToSimplifiedCancellationTokenSource,
            FileProcessStatuses,
            ChineseTransform.FileToSimplified,
            Logger
        );
    }

    /// <summary>
    /// 转换一个文件为繁体
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    [NoException]
    private async Task FileToTraditional(string filename) {
        await FileProcessUtils.ProcessOneFileAsync(
            filename,
            SaveFileDialog,
            ToTraditionalCancellationTokenSource,
            FileProcessStatuses,
            ChineseTransform.FileToTraditional,
            Logger
        );
    }

    /// <summary>
    /// 转换多个文件为简体
    /// </summary>
    /// <param name="filenames"></param>
    /// <returns></returns>
    private async Task MultiFilesToSimplified(ICollection<string> filenames) {
        await FileProcessUtils.ProcessMultiFilesAsync(
            filenames,
            SaveDirectoryDialog,
            ToSimplifiedCancellationTokenSource,
            FileProcessStatuses,
            ChineseTransform.FileToSimplified,
            Window,
            Logger
        );
    }

    /// <summary>
    /// 转换多个文件为繁体
    /// </summary>
    /// <param name="filenames"></param>
    /// <returns></returns>
    private async Task MultiFilesToTraditional(ICollection<string> filenames) {
        await FileProcessUtils.ProcessMultiFilesAsync(
            filenames,
            SaveDirectoryDialog,
            ToTraditionalCancellationTokenSource,
            FileProcessStatuses,
            ChineseTransform.FileToTraditional,
            Window,
            Logger
        );
    }

    /// <summary>
    /// 转简体
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ToSimplifiedClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 正在工作
        if (IsToSimplifiedWorking) {
            return;
        }
        var hasFile = DragDropTextBox.HasFile;
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, hasFile, DragDropTextBox.FileNames)) {
            return;
        }
        // 处理文本
        if (!hasFile) {
            StringToSimplified();
            return;
        }
        ToSimplifiedCancellationTokenSource.Dispose();
        ToSimplifiedCancellationTokenSource = new();
        IsToSimplifiedWorking = true;
        await FileToSimplified();
        IsToSimplifiedWorking = false;
    }

    /// <summary>
    /// 转繁体
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ToTraditionalClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 正在工作
        if (IsToTraditionalWorking) {
            return;
        }
        var hasFile = DragDropTextBox.HasFile;
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, hasFile, DragDropTextBox.FileNames)) {
            return;
        }
        // 处理文本
        if (!hasFile) {
            StringToTraditional();
            return;
        }
        ToTraditionalCancellationTokenSource.Dispose();
        ToTraditionalCancellationTokenSource = new();
        IsToTraditionalWorking = true;
        await FileToTraditional();
        IsToTraditionalWorking = false;
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        OutputText = string.Empty;
        // 没有正在处理的任务
        if (!IsToSimplifiedWorking && !IsToTraditionalWorking) {
            FileProcessStatuses.Clear();
        }
        DragDropTextBox.Clear();
    }

    /// <summary>
    /// 取消转繁体
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelToTraditionalClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        ToTraditionalCancellationTokenSource.Cancel();
    }

    /// <summary>
    /// 取消转简体
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelToSimplifiedClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        ToSimplifiedCancellationTokenSource.Cancel();
    }
}
