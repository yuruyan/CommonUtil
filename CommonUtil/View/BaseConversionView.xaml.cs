namespace CommonUtil.View;

public partial class BaseConversionView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty BaseOptionsProperty = DependencyProperty.Register("BaseOptions", typeof(List<int>), typeof(BaseConversionView), new PropertyMetadata());
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(BaseConversionView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(BaseConversionView), new PropertyMetadata(""));
    public static readonly DependencyProperty SourceBaseIndexProperty = DependencyProperty.Register("SourceBaseIndex", typeof(int), typeof(BaseConversionView), new PropertyMetadata(8));
    public static readonly DependencyProperty TargetBaseIndexProperty = DependencyProperty.Register("TargetBaseIndex", typeof(int), typeof(BaseConversionView), new PropertyMetadata(8));

    /// <summary>
    /// 进制选择
    /// </summary>
    public List<int> BaseOptions {
        get { return (List<int>)GetValue(BaseOptionsProperty); }
        set { SetValue(BaseOptionsProperty, value); }
    }
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
        BaseOptions = new();
        for (int i = 2; i <= 36; i++) {
            BaseOptions.Add(i);
        }
        InitializeComponent();
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
        string[] numbers = CommonUtils.NormalizeMultipleLineText(InputText).Split('\n');
        foreach (var number in numbers) {
            if (string.IsNullOrEmpty(number.Trim())) {
                sb.Append('\n');
                continue;
            }
            try {
                sb.Append($"{BaseConversion.ConvertFromDecimal(BaseConversion.ConvertToDecimal(number, SourceBaseIndex + BaseOptions[0]), TargetBaseIndex + BaseOptions[0])}\n");
            } catch {
                //Logger.Info(error);
                sb.Append('\n');
                //OutputText = string.Empty; // 清空输入
                //MessageBoxUtils.Error("转换失败！");
                //return;
            }
        }
        OutputText = sb.ToString();
    }

}

