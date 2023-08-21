namespace CommonUtil.View;

public partial class MarkdownTableConversionView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IDictionary<string, string> SplitTextDict;
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(MarkdownTableConversionView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(MarkdownTableConversionView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty SplitTextOptionsProperty = DependencyProperty.Register("SplitTextOptions", typeof(List<string>), typeof(MarkdownTableConversionView), new PropertyMetadata());

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
    /// 分隔文本选项
    /// </summary>
    public List<string> SplitTextOptions {
        get { return (List<string>)GetValue(SplitTextOptionsProperty); }
        set { SetValue(SplitTextOptionsProperty, value); }
    }

    public MarkdownTableConversionView() : base(ResponsiveMode.Variable) {
        SplitTextDict = new Dictionary<string, string>(DataSet.MarkdownTableConversionSplitOptionDict);
        SplitTextOptions = new(SplitTextDict.Keys);
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
    }

    /// <summary>
    /// 生成表格
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GenerateTablerClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (string.IsNullOrEmpty(InputText)) {
            MessageBoxUtils.Info("请输入文本");
            return;
        }
        OutputText = MarkdownTableConversion.ConvertToTable(InputText, GetComboBoxText());
    }

    /// <summary>
    /// 获取 ComboBox 文本
    /// </summary>
    /// <returns></returns>
    private string GetComboBoxText() {
        object selectedValue = SplitTextComboBox.SelectedValue;
        string text = SplitTextComboBox.Text;
        // 非用户输入
        if (selectedValue != null) {
            if (selectedValue is string t) {
                text = SplitTextDict[t];
            }
        }
        return text;
    }
}
