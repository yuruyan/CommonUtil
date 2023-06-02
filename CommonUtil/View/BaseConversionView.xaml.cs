namespace CommonUtil.View;

public partial class BaseConversionView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly IReadOnlyList<int> BaseOptions = Enumerable.Range(2, 35).ToList();
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(BaseConversionView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(BaseConversionView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty SourceBaseIndexProperty = DependencyProperty.Register("SourceBaseIndex", typeof(int), typeof(BaseConversionView), new PropertyMetadata(8));
    public static readonly DependencyProperty TargetBaseIndexProperty = DependencyProperty.Register("TargetBaseIndex", typeof(int), typeof(BaseConversionView), new PropertyMetadata(8));

    /// <summary>
    /// 输入
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }
    /// <summary>
    /// 输出
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }
    /// <summary>
    /// 源进制索引
    /// </summary>
    public int SourceBaseIndex {
        get { return (int)GetValue(SourceBaseIndexProperty); }
        set { SetValue(SourceBaseIndexProperty, value); }
    }
    /// <summary>
    /// 目标进制索引
    /// </summary>
    public int TargetBaseIndex {
        get { return (int)GetValue(TargetBaseIndexProperty); }
        set { SetValue(TargetBaseIndexProperty, value); }
    }

    public BaseConversionView() {
        InitializeComponent();
        BaseOptionsToComboBox.ItemsSource = BaseOptionsFromComboBox.ItemsSource = BaseOptions;
    }

    /// <summary>
    /// 键盘事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InputKeyUp(object sender, KeyEventArgs e) {
        e.Handled = true;
        ConvertNumber();
    }

    /// <summary>
    /// 进制选择改变
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BaseSelectionChanged(object sender, SelectionChangedEventArgs e) {
        ConvertNumber();
    }

    /// <summary>
    /// 转换
    /// </summary>
    private void ConvertNumber() {
        var sb = new StringBuilder();
        string[] numbers = InputText.ReplaceLineFeedWithLinuxStyle().Split('\n');
        foreach (var number in numbers) {
            if (string.IsNullOrEmpty(number.Trim())) {
                sb.Append('\n');
                continue;
            }
            try {
                sb.Append($"{BaseConversion.ConvertFromDecimal(BaseConversion.ConvertToDecimal(number, SourceBaseIndex + BaseOptions[0]), TargetBaseIndex + BaseOptions[0])}\n");
            } catch {
                sb.Append('\n');
            }
        }
        OutputText = sb.ToString();
    }
}
