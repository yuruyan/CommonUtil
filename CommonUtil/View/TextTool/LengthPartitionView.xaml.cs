namespace CommonUtil.View;

public partial class LengthPartitionView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(LengthPartitionView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(LengthPartitionView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(LengthPartitionView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(LengthPartitionView), new PropertyMetadata(false));
    public static readonly DependencyProperty PartitionLengthProperty = DependencyProperty.Register("PartitionLength", typeof(double), typeof(LengthPartitionView), new PropertyMetadata(1.0));
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
    /// 拆分长度
    /// </summary>
    public double PartitionLength {
        get { return (double)GetValue(PartitionLengthProperty); }
        set { SetValue(PartitionLengthProperty, value); }
    }

    public LengthPartitionView() : base(ResponsiveMode.Variable) {
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

    private void TextPartitionClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        int length = (int)PartitionLength;
        if (length < 1) {
            length = 1;
        }
        OutputText = string.Join('\n', LengthPartition.Split(InputText, length));
    }

    /// <summary>
    /// 转换浮点数为整数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumberBoxLostFocusHandler(object sender, RoutedEventArgs e) {
        MiscUtils.NumberBoxDoubleToIntLostFocusHandler(sender, e);
    }
}
