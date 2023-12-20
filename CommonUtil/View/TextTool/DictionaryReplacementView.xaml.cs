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
        OutputText = string.Empty;
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
}
