namespace CommonUtil.View;

public partial class PunctuationReplacementView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(PunctuationReplacementView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(PunctuationReplacementView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(PunctuationReplacementView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(PunctuationReplacementView), new PropertyMetadata(false));
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

    public PunctuationReplacementView() {
        InitializeComponent();
    }

    /// <summary>
    /// 处理
    /// </summary>
    /// <param name="ChineseToEnglish">是否中文标点转英文</param>
    private async void TextProcess(bool ChineseToEnglish) {
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }

        // 文本处理
        if (!HasFile) {
            StringTextProcess(ChineseToEnglish ? TextTool.ReplaceChinesePunctuationWithEnglish : TextTool.ReplaceEnglishPunctuationWithChinese);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            $"{nameof(PunctuationReplacementView)}|{nameof(TextProcess)}|{GetHashCode()}",
            () => FileTextProcess(ChineseToEnglish ? TextTool.FileReplaceChinesePunctuationWithEnglish : TextTool.FileReplaceEnglishPunctuationWithChinese)
        );
    }

    private void ReplaceWithChinesePunctuationClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        TextProcess(false);
    }

    private void ReplaceWithEnglishPunctuationClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        TextProcess(true);
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    /// <param name="func"></param>
    private void StringTextProcess(TextTool.TextProcess func) {
        OutputText = func(InputText);
    }

    /// <summary>
    /// 文件文本处理
    /// </summary>
    /// <param name="func"></param>
    private async Task FileTextProcess(TextTool.FileProcess func) {
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
            args: new object[] { inputPath, outputPath }
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
