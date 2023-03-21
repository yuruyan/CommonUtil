namespace CommonUtil.View;

public partial class JsonExtractorView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(JsonExtractorView), new PropertyMetadata(""));
    public static readonly DependencyProperty PatternTextProperty = DependencyProperty.Register("PatternText", typeof(string), typeof(JsonExtractorView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(JsonExtractorView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(JsonExtractorView), new PropertyMetadata(false));
    public static readonly DependencyProperty ResultListProperty = DependencyProperty.Register("ResultList", typeof(ExtendedObservableCollection<IList<string>>), typeof(JsonExtractorView), new PropertyMetadata());
    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "CSV|*.csv"
    };

    /// <summary>
    /// 输入文本
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
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
    /// 提取模式
    /// </summary>
    public string PatternText {
        get { return (string)GetValue(PatternTextProperty); }
        set { SetValue(PatternTextProperty, value); }
    }
    /// <summary>
    /// 结果集
    /// </summary>
    public ExtendedObservableCollection<IList<string>> ResultList {
        get { return (ExtendedObservableCollection<IList<string>>)GetValue(ResultListProperty); }
        set { SetValue(ResultListProperty, value); }
    }

    public JsonExtractorView() {
        ResultList = new();
        InputText = Resource.Resource.JsonExtractorViewDemoJson;
        PatternText = Resource.Resource.JsonExtractorViewDemoPattern;
        InitializeComponent();
    }

    /// <summary>
    /// 处理数据提取
    /// </summary>
    private async void HandleExtract() {
        // 二进制文件警告
        if (HasFile && CommonUtils.IsLikelyBinaryFile(FileName)) {
            WarningDialog dialog = WarningDialog.Shared;
            dialog.DetailText = "文件可能是二进制文件，是否继续？";
            if (await dialog.ShowAsync() != ModernWpf.Controls.ContentDialogResult.Primary) {
                return;
            }
        }
        // 检查输入
        if (!HasFile) {
            if (!UIUtils.CheckInputNullOrEmpty(new KeyValuePair<string?, string>[] {
                new (InputText, "请输入文本"),
                new (PatternText, "请输入提取模式"),
            })) {
                return;
            }
        }

        ResultDetailTextBlock.Visibility = Visibility.Visible;
        if (!ThrottleUtils.CheckStateAndSet(HandleExtract)) {
            return;
        }
        try {
            // 数据提取
            if (!HasFile) {
                StringExtract();
                return;
            }
            await FileExtract();
        } catch (JsonException) {
            MessageBoxUtils.Error("Json 解析失败");
        } catch (PatternParseException) {
            MessageBoxUtils.Error("提取模式解析失败");
        } catch {
            MessageBoxUtils.Error("失败");
        } finally {
            ThrottleUtils.SetFinished(HandleExtract);
        }
    }

    /// <summary>
    /// 文本提取
    /// </summary>
    private void StringExtract() {
        var patterns = ParsePattern();
        ResultList = new(JsonExtractor.Extract(InputText, patterns).Transpose());
        ResultListGridView.Columns.Clear();
        var headers = JsonExtractor.GetPatternHeaders(patterns);
        int index = 0;
        foreach (var header in headers) {
            ResultListGridView.Columns.Add(new() {
                Header = header,
                DisplayMemberBinding = new Binding($"[{index++}]")
            });
        }
    }

    /// <summary>
    /// 解析输入的 Pattern
    /// </summary>
    /// <returns></returns>
    private string[] ParsePattern() {
        return PatternText.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// 文件文本提取
    /// </summary>
    private async Task FileExtract() {
        var patterns = ParsePattern();
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await UIUtils.CreateFileProcessTask(
            (Action<string, string, IEnumerable<string>>)JsonExtractor.FileExtract,
            outputPath,
            showErrorInfo: false,
            reThrowError: true,
            args: new object[] { inputPath, outputPath, patterns }
        );
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        CopyResultViewItems(ResultList);
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        FileName = InputText = string.Empty;
        ResultList = new();
        ResultDetailTextBlock.Visibility = Visibility.Collapsed;
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

    /// <summary>
    /// 提取数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void JsonExtractClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        HandleExtract();
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        CopyResultViewItems(ResultListView.SelectedItems.Cast<IList<string>>());
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// 复制结果集
    /// </summary>
    /// <param name="data"></param>
    private static void CopyResultViewItems(IEnumerable<IEnumerable<string>> data) {
        var copyData = new List<string>();
        foreach (var item in data) {
            copyData.Add(string.Join('\t', item));
        }
        Clipboard.SetDataObject(string.Join('\n', copyData));
    }
}
