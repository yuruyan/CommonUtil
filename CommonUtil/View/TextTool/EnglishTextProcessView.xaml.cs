﻿namespace CommonUtil.View;

public partial class EnglishTextProcessView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(EnglishTextProcessView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(EnglishTextProcessView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(EnglishTextProcessView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(EnglishTextProcessView), new PropertyMetadata(false));
    public static readonly DependencyProperty ProcessPatternDictProperty = DependencyProperty.Register("ProcessPatternDict", typeof(IDictionary<string, (TextTool.TextProcess, TextTool.FileProcess)>), typeof(EnglishTextProcessView), new PropertyMetadata());
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
    /// <summary>
    /// 处理模式
    /// </summary>
    public IDictionary<string, (TextTool.TextProcess, TextTool.FileProcess)> ProcessPatternDict {
        get { return (IDictionary<string, (TextTool.TextProcess, TextTool.FileProcess)>)GetValue(ProcessPatternDictProperty); }
        private set { SetValue(ProcessPatternDictProperty, value); }
    }

    public EnglishTextProcessView() : base(ResponsiveMode.Variable) {
        ProcessPatternDict = new Dictionary<string, (TextTool.TextProcess, TextTool.FileProcess)>(DataSet.EnglishTextProcessOptionDict);
        InitializeComponent();
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
    /// 文本处理
    /// </summary>
    /// <param name="func"></param>
    private void StringTextProcess(TextTool.TextProcess func) {
        OutputText = func(InputText);
    }

    /// <summary>
    /// 文件处理
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
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

        var (textProcess, fileProcess) = ProcessPatternDict[
            CommonUtils.NullCheck(ProcessPatternComboBox.SelectedValue.ToString())
        ];

        // 文本处理
        if (!HasFile) {
            StringTextProcess(textProcess);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            $"{nameof(EnglishTextProcessView)}|{nameof(TextProcessClick)}|{GetHashCode()}",
            () => FileTextProcess(fileProcess)
        );
    }
}
