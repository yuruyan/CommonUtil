namespace CommonUtil.View;

public partial class EnglishWordBracesView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(EnglishWordBracesView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(EnglishWordBracesView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(EnglishWordBracesView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(EnglishWordBracesView), new PropertyMetadata(false));
    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "文本文件|*.txt|All Files|*.*"
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
    /// 是否有文件
    /// </summary>
    public bool HasFile {
        get { return (bool)GetValue(HasFileProperty); }
        set { SetValue(HasFileProperty, value); }
    }
    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName {
        get { return (string)GetValue(FileNameProperty); }
        set { SetValue(FileNameProperty, value); }
    }

    public EnglishWordBracesView() {
        InitializeComponent();
    }

    /// <summary>
    /// 添加两边空格
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void AddBracesClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }
        bool includeNumber = IncludeNumberCheckBox.IsChecked ?? false;

        // 文本处理
        if (!HasFile) {
            StringTextProcess(EnglishWordProcess.AddEnglishWordBraces, includeNumber);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            $"{nameof(EnglishWordBracesView)}|{nameof(AddBracesClickHandler)}|{GetHashCode()}",
            () => FileTextProcess(EnglishWordProcess.FileAddEnglishWordBraces, includeNumber)
        );
    }

    /// <summary>
    /// 移除两边空格
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void RemoveBracesClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }
        bool includeNumber = IncludeNumberCheckBox.IsChecked ?? false;

        // 文本处理
        if (!HasFile) {
            StringTextProcess(EnglishWordProcess.RemoveEnglishWordBraces, includeNumber);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            $"{nameof(EnglishWordBracesView)}|{nameof(RemoveBracesClickHandler)}|{GetHashCode()}",
            () => FileTextProcess(EnglishWordProcess.FileRemoveEnglishWordBraces, includeNumber)
        );
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    /// <param name="func"></param>
    /// <param name="includeNumber"></param>
    private void StringTextProcess(Func<string, bool, string> func, bool includeNumber) {
        OutputText = func(InputText, includeNumber);
    }

    /// <summary>
    /// 文件文本处理
    /// </summary>
    /// <param name="func"></param>
    /// <param name="includeNumber"></param>
    private async Task FileTextProcess(Action<string, string, bool> func, bool includeNumber) {
        var text = InputText;
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await UIUtils.CreateFileProcessTask(
            func,
            outputPath,
            args: new object[] { inputPath, outputPath, includeNumber }
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
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        InputText = OutputText = string.Empty;
        DragDropTextBox.Clear();
    }

    /// <summary>
    /// DragDropEvent
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DragDropEventHandler(object sender, object e) {
        if (e is IEnumerable<string> array) {
            if (!array.Any()) {
                return;
            }
            // 判断是否为文件
            if (File.Exists(array.First())) {
                FileName = array.First();
            } else {
                DragDropTextBox.Clear();
            }
        }
    }

}
