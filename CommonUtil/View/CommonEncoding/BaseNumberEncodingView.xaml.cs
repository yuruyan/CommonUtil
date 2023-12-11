namespace CommonUtil.View;

public partial class BaseNumberEncodingView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(BaseNumberEncodingView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(BaseNumberEncodingView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty ConversionOptionsProperty = DependencyProperty.Register("ConversionOptions", typeof(Dictionary<string, (BaseNumberStringConverter.ConvertFromNumber, BaseNumberStringConverter.ConvertToNumber)>), typeof(BaseNumberEncodingView), new PropertyMetadata());
    public static readonly DependencyProperty SelectedConversionOptionProperty = DependencyProperty.Register("SelectedConversionOption", typeof(string), typeof(BaseNumberEncodingView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IsPaddingLeftProperty = DependencyProperty.Register("IsPaddingLeft", typeof(bool), typeof(BaseNumberEncodingView), new PropertyMetadata(true));

    /// <summary>
    /// 选中的 ConversionOption
    /// </summary>
    public string SelectedConversionOption {
        get { return (string)GetValue(SelectedConversionOptionProperty); }
        set { SetValue(SelectedConversionOptionProperty, value); }
    }
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
    /// 左侧填充
    /// </summary>
    public bool IsPaddingLeft {
        get { return (bool)GetValue(IsPaddingLeftProperty); }
        set { SetValue(IsPaddingLeftProperty, value); }
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
    private void EncodeClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (string.IsNullOrEmpty(InputText)) {
            MessageBoxUtils.Info("请输入文本");
            return;
        }
        if (!ConversionOptions.TryGetValue(SelectedConversionOption, out var method)) {
            return;
        }

        try {
            OutputText = method.Item2(InputText, IsPaddingLeft) ?? string.Empty;
        } catch (Exception error) {
            Logger.Info(error);
            MessageBoxUtils.Error("编码失败");
        }
    }

    /// <summary>
    /// 解码
    /// </summary>
    private void DecodeClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (string.IsNullOrEmpty(InputText)) {
            MessageBoxUtils.Info("请输入文本");
            return;
        }
        if (!ConversionOptions.TryGetValue(SelectedConversionOption, out var method)) {
            return;
        }

        try {
            OutputText = method.Item1(InputText) ?? string.Empty;
        } catch (Exception error) {
            Logger.Info(error);
            MessageBoxUtils.Error("解码失败");
        }
    }
}

