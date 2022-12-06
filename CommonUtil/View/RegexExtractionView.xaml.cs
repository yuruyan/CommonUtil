using Microsoft.Win32;

namespace CommonUtil.View;

public partial class RegexExtractionView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(RegexExtractionView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RegexExtractionView), new PropertyMetadata(""));
    public static readonly DependencyProperty SearchRegexProperty = DependencyProperty.Register("SearchRegex", typeof(string), typeof(RegexExtractionView), new PropertyMetadata(""));
    public static readonly DependencyProperty ExtractionPatternProperty = DependencyProperty.Register("ExtractionPattern", typeof(string), typeof(RegexExtractionView), new PropertyMetadata("\\0"));
    public static readonly DependencyProperty IgnoreCaseProperty = DependencyProperty.Register("IgnoreCase", typeof(bool), typeof(RegexExtractionView), new PropertyMetadata(true));
    public static readonly DependencyProperty MatchListProperty = DependencyProperty.Register("MatchList", typeof(IList<string>), typeof(RegexExtractionView), new PropertyMetadata());
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(RegexExtractionView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(RegexExtractionView), new PropertyMetadata(false));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RegexExtractionView), new PropertyMetadata(true));
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
    /// 查找正则
    /// </summary>
    public string SearchRegex {
        get { return (string)GetValue(SearchRegexProperty); }
        set { SetValue(SearchRegexProperty, value); }
    }
    /// <summary>
    /// 提取模式
    /// </summary>
    public string ExtractionPattern {
        get { return (string)GetValue(ExtractionPatternProperty); }
        set { SetValue(ExtractionPatternProperty, value); }
    }
    /// <summary>
    /// 忽略大小写
    /// </summary>
    public bool IgnoreCase {
        get { return (bool)GetValue(IgnoreCaseProperty); }
        set { SetValue(IgnoreCaseProperty, value); }
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
    /// 匹配列表
    /// </summary>
    public IList<string> MatchList {
        get { return (IList<string>)GetValue(MatchListProperty); }
        set { SetValue(MatchListProperty, value); }
    }
    /// <summary>
    /// 是否扩宽
    /// </summary>
    public bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }
    /// <summary>
    /// 常用正则表达式 Dialog
    /// </summary>
    private CommonRegexListDialog? CommonRegexListDialog;

    public RegexExtractionView() {
        MatchList = Array.Empty<string>();
        InitializeComponent();
        // 响应式布局
        UIUtils.SetLoadedOnceEventHandler(this, (_, _) => {
            Window window = Window.GetWindow(this);
            double expansionThreshold = (double)Resources["ExpansionThreshold"];
            IsExpanded = window.ActualWidth >= expansionThreshold;
            DependencyPropertyDescriptor
                .FromProperty(Window.ActualWidthProperty, typeof(Window))
                .AddValueChanged(window, (_, _) => {
                    IsExpanded = window.ActualWidth >= expansionThreshold;
                });
        });
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
        InputText = OutputText = string.Empty;
        MatchList = Array.Empty<string>();
        ResultDetailTextBlock.Visibility = Visibility.Collapsed;
        DragDropTextBox.Clear();
    }

    /// <summary>
    /// 查找结果
    /// </summary>
    private async void SearchResult() {
        // 检验输入
        if (!UIUtils.CheckInputNullOrEmpty(new KeyValuePair<string?, string>[] {
            new (SearchRegex,  "查找正则不能为空"),
            new (ExtractionPattern,  "提取模式不能为空"),
        })) {
            return;
        }
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }

        ResultDetailTextBlock.Visibility = Visibility.Visible;

        // 文本处理
        if (!HasFile) {
            StringExtract();
            return;
        }
        ThrottleUtils.ThrottleAsync(SearchResult, FileExtract);
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    private void StringExtract() {
        var list = RegexExtraction.Extract(
            SearchRegex,
            InputText,
            ExtractionPattern,
            ignoreCase: IgnoreCase
        );
        if (list is null) {
            MessageBox.Error("正则表达式有误");
            return;
        }
        MatchList = list;
        OutputText = string.Join('\n', list);
    }

    /// <summary>
    /// 文件文本处理
    /// </summary>
    /// <returns></returns>
    private async Task FileExtract() {
        var inputPath = FileName;
        var searchRegex = SearchRegex;
        var extractionPattern = ExtractionPattern;
        var ignoreCase = IgnoreCase;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        try {
            await UIUtils.CreateFileProcessTask(
                RegexExtraction.FileExtract,
                outputPath,
                reThrowError: true,
                args: new object[] { inputPath, outputPath, searchRegex, extractionPattern, ignoreCase }
            );
        } catch (ArgumentException) {
            MessageBox.Error("正则表达式有误");
        } catch { }
    }

    /// <summary>
    /// 点击查找
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SearchClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        SearchResult();
    }

    /// <summary>
    /// 按下回车查找
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SearchRegexComboBoxKeyUp(object sender, KeyEventArgs e) {
        e.Handled = true;
        // todo bug：按下Enter键保存文件后又会自动触发事件
        //if (e.Key == Key.Enter) {
        //    SearchResult();
        //}
    }

    /// <summary>
    /// 显示更多正则表达式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MoreRegexMouseUp(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        CommonRegexListDialog ??= new();
        CommonRegexListDialog.ShowAsync();
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
