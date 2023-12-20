using Csv;

namespace CommonUtil.View;

public partial class DictionaryReplacementView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(DictionaryReplacementView), new PropertyMetadata(""));
    public static readonly DependencyProperty HasDataFileProperty = DependencyProperty.Register("HasDataFile", typeof(bool), typeof(DictionaryReplacementView), new PropertyMetadata(false));
    public static readonly DependencyProperty DataFileNameProperty = DependencyProperty.Register("DataFileName", typeof(string), typeof(DictionaryReplacementView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty DataInputTextProperty = DependencyProperty.Register("DataInputText", typeof(string), typeof(DictionaryReplacementView), new PropertyMetadata(""));
    public static readonly DependencyProperty HasDictFileProperty = DependencyProperty.Register("HasDictFile", typeof(bool), typeof(DictionaryReplacementView), new PropertyMetadata(false));
    public static readonly DependencyProperty DictFileNameProperty = DependencyProperty.Register("DictFileName", typeof(string), typeof(DictionaryReplacementView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty DictInputTextProperty = DependencyProperty.Register("DictInputText", typeof(string), typeof(DictionaryReplacementView), new PropertyMetadata(""));

    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "文本文件|*.txt|All Files|*.*"
    };

    /// <summary>
    /// 输出文本
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }
    /// <summary>
    /// 输入文本
    /// </summary>
    public string DataInputText {
        get { return (string)GetValue(DataInputTextProperty); }
        set { SetValue(DataInputTextProperty, value); }
    }
    /// <summary>
    /// 是否有文件
    /// </summary>
    public bool HasDataFile {
        get { return (bool)GetValue(HasDataFileProperty); }
        set { SetValue(HasDataFileProperty, value); }
    }
    /// <summary>
    /// 文件名
    /// </summary>
    public string DataFileName {
        get { return (string)GetValue(DataFileNameProperty); }
        set { SetValue(DataFileNameProperty, value); }
    }
    /// <summary>
    /// 字典文本
    /// </summary>
    public string DictInputText {
        get { return (string)GetValue(DictInputTextProperty); }
        set { SetValue(DictInputTextProperty, value); }
    }
    /// <summary>
    /// 是否有文件
    /// </summary>
    public bool HasDictFile {
        get { return (bool)GetValue(HasDictFileProperty); }
        set { SetValue(HasDictFileProperty, value); }
    }
    /// <summary>
    /// 文件名
    /// </summary>
    public string DictFileName {
        get { return (string)GetValue(DictFileNameProperty); }
        set { SetValue(DictFileNameProperty, value); }
    }

    public DictionaryReplacementView() : base(ResponsiveMode.Variable) {
        InitializeComponent();
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        DataInputText = DictInputText = OutputText = string.Empty;
        DragDropDataTextBox.Clear();
        DragDropDictTextBox.Clear();
    }

    /// <summary>
    /// DragDropEvent
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DragDropEventHandler(object sender, object e) {
        if (sender is not FrameworkElement element) {
            return;
        }
        if (e is not IEnumerable<string> array) {
            return;
        }
        if (array.FirstOrDefault() is not string filename) {
            return;
        }

        if (element.Name == "DragDropDataTextBox") {
            // 判断是否为文件
            if (File.Exists(filename)) {
                DataFileName = filename;
            } else {
                DragDropDataTextBox.Clear();
            }
        } else if (element.Name == "DragDropDictTextBox") {
            // 判断是否为文件
            if (File.Exists(filename)) {
                DictFileName = filename;
            } else {
                DragDropDictTextBox.Clear();
            }
        }
    }

    /// <summary>
    /// 处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void TextProcessClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(DataInputText, HasDataFile, DataFileName)) {
            return;
        }
        if (!await UIUtils.CheckTextAndFileInputAsync(DictInputText, HasDictFile, DictFileName)) {
            return;
        }
        // 文本处理
        if (!HasDataFile) {
            StringTextProcess();
            return;
        }
        ThrottleUtils.ThrottleAsync(
            $"{nameof(DictionaryReplacementView)}|{nameof(TextProcessClickHandler)}|{GetHashCode()}",
            FileTextProcess
        );
    }

    /// <summary>
    /// 文件处理
    /// </summary>
    /// <returns></returns>
    private async Task FileTextProcess() {
        var inputPath = DataFileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await UIUtils.CreateFileProcessTask(
            DictionaryReplacement.FileReplaceAggregate,
            outputPath,
            args: new object[] { inputPath, outputPath, GetReplacementDictionary() }
        );
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    private void StringTextProcess() {
        OutputText = DictionaryReplacement.ReplaceAggregate(DataInputText, GetReplacementDictionary());
    }

    /// <summary>
    /// 获取 ReplacementDictionary
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, string> GetReplacementDictionary() {
        var dict = new Dictionary<string, string>();
        if (HasDictFile) {
            dict = ParseCSV(TaskUtils.Try(() => File.ReadAllText(DictFileName), string.Empty)!);
        } else {
            dict = ParseCSV(DictInputText);
        }
        return dict;
    }

    /// <summary>
    /// 解析 CSV
    /// </summary>
    /// <param name="csvText"></param>
    /// <returns></returns>
    private static Dictionary<string, string> ParseCSV(string csvText) {
        var dict = new Dictionary<string, string>();
        foreach (var line in CsvReader.ReadFromText(csvText)) {
            if (line.Values.Length >= 2) {
                dict[line.Values[0]] = line.Values[1];
            }
        }
        // 移除空串
        dict.Remove(string.Empty);
        return dict;
    }
}
