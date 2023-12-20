namespace CommonUtil.View;

public partial class DictionaryReplacementView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(DictionaryReplacementView), new PropertyMetadata(""));
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

}
