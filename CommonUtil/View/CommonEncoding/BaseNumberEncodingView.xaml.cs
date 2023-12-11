namespace CommonUtil.View;

public partial class BaseNumberEncodingView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(BaseNumberEncodingView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(BaseNumberEncodingView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty ConversionOptionsProperty = DependencyProperty.Register("ConversionOptions", typeof(Dictionary<string, (BaseNumberStringConverter.ConvertFromNumber, BaseNumberStringConverter.ConvertToNumber)>), typeof(BaseNumberEncodingView), new PropertyMetadata());

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
    /// 转换模式
    /// </summary>
    public Dictionary<string, (BaseNumberStringConverter.ConvertFromNumber, BaseNumberStringConverter.ConvertToNumber)> ConversionOptions {
        get { return (Dictionary<string, (BaseNumberStringConverter.ConvertFromNumber, BaseNumberStringConverter.ConvertToNumber)>)GetValue(ConversionOptionsProperty); }
        set { SetValue(ConversionOptionsProperty, value); }
    }

    public BaseNumberEncodingView() : base(ResponsiveMode.Variable) {
        ConversionOptions = new(DataSet.BaseNumberConversionOptionDict);
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
    /// 编码
    /// </summary>
    private void EncodingClickHandler() {
        try {
        } catch (Exception error) {
            Logger.Error(error);
            MessageBoxUtils.Error("编码失败");
        }
    }

    /// <summary>
    /// 编码
    /// </summary>
    private void EncodeClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;

    }

    /// <summary>
    /// 解码
    /// </summary>
    private void DecodeClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;

    }
}

