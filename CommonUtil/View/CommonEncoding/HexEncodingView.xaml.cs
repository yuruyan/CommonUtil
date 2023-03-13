namespace CommonUtil.View;

public partial class HexEncodingView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(HexEncodingView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(HexEncodingView), new PropertyMetadata(""));

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

    public HexEncodingView() {
        InitializeComponent();
    }

    /// <summary>
    /// 编码
    /// </summary>
    private void EncodingClick() {
        try {
            OutputText = CommonEncoding.HexEncode(InputText);
        } catch (Exception error) {
            Logger.Error(error);
            MessageBoxUtils.Error("编码失败");
        }
    }

    /// <summary>
    /// 解码
    /// </summary>
    private void DecodingClick() {
        try {
            OutputText = CommonEncoding.HexDecode(InputText);
        } catch (Exception error) {
            Logger.Error(error);
            MessageBoxUtils.Error("解码失败");
        }
    }
}

