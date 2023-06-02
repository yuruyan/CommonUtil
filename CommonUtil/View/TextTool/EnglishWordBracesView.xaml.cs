namespace CommonUtil.View;

public partial class EnglishWordBracesView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly IReadOnlyList<KeyValuePair<string, EnglishWordBracesMode>> EnglishWordBracesModes = new List<KeyValuePair<string, EnglishWordBracesMode>> {
        new ("默认", EnglishWordBracesMode.Default),
        new ("英文包括数字", EnglishWordBracesMode.IncludeNumber),
        new ("英文包括ASCII", EnglishWordBracesMode.IncludeASCII),
    };
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

    public EnglishWordBracesView() : base(ResponsiveMode.Variable) {
        InitializeComponent();
        this.SetLoadedOnceEventHandler((_, _) => {
            EnglishWordBracesModeComboBox.SelectedIndex = 0;
        });
    }

    /// <summary>
    /// 添加两边空格
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void AddBracesClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (EnglishWordBracesModeComboBox.SelectedValue is not EnglishWordBracesMode mode) {
            return;
        }
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }

        // 文本处理
        if (!HasFile) {
            StringTextProcess(EnglishWordBraces.AddEnglishWordBraces, mode);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            $"{nameof(EnglishWordBracesView)}|{nameof(AddBracesClickHandler)}|{GetHashCode()}",
            () => FileTextProcess(EnglishWordBraces.FileAddEnglishWordBraces, mode)
        );
    }

    /// <summary>
    /// 移除两边空格
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void RemoveBracesClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (EnglishWordBracesModeComboBox.SelectedValue is not EnglishWordBracesMode mode) {
            return;
        }
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }

        // 文本处理
        if (!HasFile) {
            StringTextProcess(EnglishWordBraces.RemoveEnglishWordBraces, mode);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            $"{nameof(EnglishWordBracesView)}|{nameof(RemoveBracesClickHandler)}|{GetHashCode()}",
            () => FileTextProcess(EnglishWordBraces.FileRemoveEnglishWordBraces, mode)
        );
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    /// <param name="func"></param>
    /// <param name="mode"></param>
    private void StringTextProcess(Func<string, EnglishWordBracesMode, string> func, EnglishWordBracesMode mode) {
        OutputText = func(InputText, mode);
    }

    /// <summary>
    /// 文件文本处理
    /// </summary>
    /// <param name="func"></param>
    /// <param name="mode"></param>
    private async Task FileTextProcess(Action<string, string, EnglishWordBracesMode> func, EnglishWordBracesMode mode) {
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
            args: new object[] { inputPath, outputPath, mode }
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
