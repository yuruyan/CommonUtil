namespace CommonUtil.View;

public partial class WhiteSpaceProcessView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(WhiteSpaceProcessView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(WhiteSpaceProcessView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(WhiteSpaceProcessView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(WhiteSpaceProcessView), new PropertyMetadata(false));
    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "文本文件|*.txt|All Files|*.*"
    };
    /// <summary>
    /// MenuChecked Array
    /// </summary>
    private readonly bool[] CheckedArray;

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

    public WhiteSpaceProcessView() : base(ResponsiveMode.Variable) {
        InitializeComponent();
        // 初始化 ProcessOptionsMenuFlyout
        DataSet.WhiteSpaceProcessOptions.ForEach(item => {
            ProcessOptionsMenuFlyout.Items.Add(new MenuItem() {
                Header = item.Item1,
            });
        });
        CheckedArray = new bool[ProcessOptionsMenuFlyout.Items.Count];
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void TextProcessClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }
        // 更新 CheckedArray
        ProcessOptionsMenuFlyout.Items
            .Cast<MenuItem>()
            .ForEach((index, item) => {
                CheckedArray[index] = item.IsChecked;
            });

        // 文本处理
        if (!HasFile) {
            TextProcessString();
            return;
        }
        ThrottleUtils.ThrottleAsync(
            $"{nameof(WhiteSpaceProcessView)}|{nameof(TextProcessClick)}|{GetHashCode()}",
            TextProcessFile
        );
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    private void TextProcessString() {
        var s = InputText;
        for (int i = 0; i < CheckedArray.Length; i++) {
            if (!CheckedArray[i]) {
                continue;
            }
            s = DataSet.WhiteSpaceProcessOptions[i].Item2(s);
        }
        OutputText = s;
    }

    /// <summary>
    /// 文件处理
    /// </summary>
    /// <returns></returns>
    private async Task TextProcessFile() {
        var text = InputText;
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await UIUtils.CreateFileProcessTask(() => {
            for (int i = 0; i < CheckedArray.Length; i++) {
                if (!CheckedArray[i]) {
                    continue;
                }
                DataSet.WhiteSpaceProcessOptions[i].Item3(inputPath, outputPath);
            }
        }, outputPath);
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
