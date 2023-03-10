namespace CommonUtil.View;

public partial class Base64ToolView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(Base64ToolView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(Base64ToolView), new PropertyMetadata(""));
    private static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(Base64ToolView), new PropertyMetadata(false));
    private static readonly DependencyProperty IsDecodeRunningProperty = DependencyProperty.Register("IsDecodeRunning", typeof(bool), typeof(Base64ToolView), new PropertyMetadata(false));
    private static readonly DependencyProperty IsEncodeRunningProperty = DependencyProperty.Register("IsEncodeRunning", typeof(bool), typeof(Base64ToolView), new PropertyMetadata(false));
    public static readonly DependencyProperty FileProcessStatusesProperty = DependencyProperty.Register("FileProcessStatuses", typeof(ObservableCollection<FileProcessStatus>), typeof(Base64ToolView), new PropertyMetadata());
    /// <summary>
    /// 保存文件对话框
    /// </summary>
    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "All Files|*.*"
    };
    /// <summary>
    /// 保存目录对话框
    /// </summary>
    private readonly VistaFolderBrowserDialog SaveDirectoryDialog = new() {
        Description = "选择保存目录",
        UseDescriptionForTitle = true
    };

    /// <summary>
    /// 输入
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }
    /// <summary>
    /// 输出结果
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }
    /// <summary>
    /// 是否扩宽
    /// </summary>
    private bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }
    /// <summary>
    /// 是否正在解码
    /// </summary>
    private bool IsDecodeRunning {
        get { return (bool)GetValue(IsDecodeRunningProperty); }
        set { SetValue(IsDecodeRunningProperty, value); }
    }
    /// <summary>
    /// 是否正在编码
    /// </summary>
    private bool IsEncodeRunning {
        get { return (bool)GetValue(IsEncodeRunningProperty); }
        set { SetValue(IsEncodeRunningProperty, value); }
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
    private CancellationTokenSource EncodeCancellationTokenSource = new();
    private CancellationTokenSource DecodeCancellationTokenSource = new();

    public Base64ToolView() {
        FileProcessStatuses = new();
        InitializeComponent();
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
    /// 解码单个文件
    /// </summary>
    [NoException]
    private async Task DecodeOneFile(string filename) {
        await FileProcessUtils.ProcessOneFileAsync(
            filename,
            SaveFileDialog,
            DecodeCancellationTokenSource,
            FileProcessStatuses,
            Base64Tool.Base64DecodeFile,
            Logger
        );
    }

    /// <summary>
    /// 编码单个文件
    /// </summary>
    /// <returns></returns>
    [NoException]
    private async Task EncodeOneFile(string filename) {
        await FileProcessUtils.ProcessOneFileAsync(
            filename,
            SaveFileDialog,
            EncodeCancellationTokenSource,
            FileProcessStatuses,
            Base64Tool.Base64EncodeFile,
            Logger
        );
    }

    /// <summary>
    /// 字符串解码
    /// </summary>
    private void DecodeString() {
        if (!UIUtils.CheckInputNullOrEmpty(InputText)) {
            return;
        }
        string? output = TaskUtils.Try(() => Base64Tool.Base64DecodeString(InputText));
        if (output is null) {
            MessageBox.Error("解码失败");
            return;
        }
        OutputText = output;
    }

    /// <summary>
    /// 字符串编码
    /// </summary>
    private void EncodeString() {
        if (!UIUtils.CheckInputNullOrEmpty(InputText)) {
            return;
        }
        string? output = TaskUtils.Try(() => Base64Tool.Base64EncodeString(InputText));
        if (output is null) {
            MessageBox.Error("编码失败");
            return;
        }
        OutputText = output;
    }

    /// <summary>
    /// 解码文件
    /// </summary>
    [NoException]
    private async Task DecodeFile() {
        var fileNames = DragDropTextBox.FileNames;
        if (fileNames.Count == 1) {
            await DecodeOneFile(fileNames[0]);
        } else {
            await DecodeMultiFiles(fileNames);
        }
    }

    /// <summary>
    /// 编码文件
    /// </summary>
    [NoException]
    private async Task EncodeFile() {
        var fileNames = DragDropTextBox.FileNames;
        if (fileNames.Count == 1) {
            await EncodeOneFile(fileNames[0]);
        } else {
            await EncodeMultiFiles(fileNames);
        }
    }

    /// <summary>
    /// 编码多个文件
    /// </summary>
    /// <param name="filenames"></param>
    /// <returns></returns>
    [NoException]
    private async Task EncodeMultiFiles(ICollection<string> filenames) {
        await FileProcessUtils.ProcessMultiFilesAsync(
            filenames,
            SaveDirectoryDialog,
            EncodeCancellationTokenSource,
            FileProcessStatuses,
            Base64Tool.Base64EncodeFile,
            Window,
            Logger
        );
    }

    /// <summary>
    /// 解码多个文件
    /// </summary>
    /// <param name="filenames"></param>
    /// <returns></returns>
    [NoException]
    private async Task DecodeMultiFiles(ICollection<string> filenames) {
        await FileProcessUtils.ProcessMultiFilesAsync(
            filenames,
            SaveDirectoryDialog,
            DecodeCancellationTokenSource,
            FileProcessStatuses,
            Base64Tool.Base64DecodeFile,
            Window,
            Logger
        );
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        MessageBox.Success("已复制");
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
        if (!IsDecodeRunning && !IsEncodeRunning) {
            FileProcessStatuses.Clear();
        }
        DragDropTextBox.Clear();
    }

    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void EncodeClickHandler(object sender, RoutedEventArgs e) {
        // 正在编码
        if (IsEncodeRunning) {
            return;
        }
        var hasFile = DragDropTextBox.HasFile;
        // 输入检查
        if (!hasFile && string.IsNullOrEmpty(InputText)) {
            MessageBox.Info("请输入文本");
            return;
        }

        // 处理文本
        if (!hasFile) {
            EncodeString();
            return;
        }
        EncodeCancellationTokenSource.Dispose();
        EncodeCancellationTokenSource = new();
        IsEncodeRunning = true;
        await EncodeFile();
        IsEncodeRunning = false;
    }

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DecodeClickHandler(object sender, RoutedEventArgs e) {
        // 正在解码
        if (IsDecodeRunning) {
            return;
        }
        var hasFile = DragDropTextBox.HasFile;
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, hasFile, DragDropTextBox.FileNames)) {
            return;
        }

        // 处理文本
        if (!hasFile) {
            DecodeString();
            return;
        }
        DecodeCancellationTokenSource.Dispose();
        DecodeCancellationTokenSource = new();
        IsDecodeRunning = true;
        await DecodeFile();
        IsDecodeRunning = false;
    }

    /// <summary>
    /// 取消解码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelDecodeClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        DecodeCancellationTokenSource.Cancel();
    }

    /// <summary>
    /// 取消编码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelEncodeClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        EncodeCancellationTokenSource.Cancel();
    }
}
