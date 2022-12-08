using CommonUtil.Store;
using Microsoft.Win32;

namespace CommonUtil.View;

public partial class RemoveDuplicateView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RemoveDuplicateView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(RemoveDuplicateView), new PropertyMetadata(""));
    public static readonly DependencyProperty SymbolOptionsProperty = DependencyProperty.Register("SymbolOptions", typeof(List<string>), typeof(RemoveDuplicateView), new PropertyMetadata());
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(RemoveDuplicateView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(RemoveDuplicateView), new PropertyMetadata(false));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RemoveDuplicateView), new PropertyMetadata(true));
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
    /// 标记选项
    /// </summary>
    public List<string> SymbolOptions {
        get { return (List<string>)GetValue(SymbolOptionsProperty); }
        set { SetValue(SymbolOptionsProperty, value); }
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
    /// 是否扩宽
    /// </summary>
    public bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }

    public RemoveDuplicateView() {
        SymbolOptions = new(DataSet.RemoveDuplicateSplitSymbolDict.Keys);
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
        DragDropTextBox.Clear();
    }

    /// <summary>
    /// 字符串去重
    /// </summary>
    /// <param name="splitSymbol"></param>
    /// <param name="mergeSymbol"></param>
    /// <param name="trim"></param>
    private void StringRemoveDuplicate(string splitSymbol, string mergeSymbol, bool trim) {
        OutputText = TextTool.RemoveDuplicate(InputText, splitSymbol, mergeSymbol, trim);
    }

    /// <summary>
    /// 文件文本去重
    /// </summary>
    /// <param name="splitSymbol"></param>
    /// <param name="mergeSymbol"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    private async Task FileRemoveDuplicate(string splitSymbol, string mergeSymbol, bool trim) {
        var text = InputText;
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await UIUtils.CreateFileProcessTask(
            TextTool.FileRemoveDuplicate,
            outputPath,
            args: new object[] { inputPath, outputPath, splitSymbol, mergeSymbol, trim }
        );
    }

    /// <summary>
    /// 文本去重
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void RemoveDuplicateClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }

        var splitSymbol = GetComboBoxText(SplitSymbolBox);
        var mergeSymbol = GetComboBoxText(MergeSymbolBox);
        var trim = TrimWhiteSpaceCheckBox.IsChecked == true;

        if (!HasFile) {
            StringRemoveDuplicate(splitSymbol, mergeSymbol, trim);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            RemoveDuplicateClick,
            () => FileRemoveDuplicate(splitSymbol, mergeSymbol, trim)
        );
    }

    /// <summary>
    /// 获取 ComboBox 文本
    /// </summary>
    /// <param name="comboBox"></param>
    /// <returns></returns>
    private string GetComboBoxText(ComboBox comboBox) {
        object selectedValue = comboBox.SelectedValue;
        string text = comboBox.Text;
        // 非用户输入
        if (selectedValue != null) {
            if (selectedValue is string t) {
                text = DataSet.RemoveDuplicateSplitSymbolDict[t];
            }
        }
        return text;
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
