namespace CommonUtil.View;

public partial class UTF8EncodingView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(UTF8EncodingView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(UTF8EncodingView), new PropertyMetadata(""));

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

    public UTF8EncodingView() {
        InitializeComponent();
    }

    /// <summary>
    /// 编码
    /// </summary>
    private void EncodingClick() {
        try {
            OutputText = CommonEncoding.UTF8Encode(InputText);
        } catch (Exception error) {
            Logger.Error(error);
            MessageBox.Error("编码失败");
        }
    }

    /// <summary>
    /// 解码
    /// </summary>
    private void DecodingClick() {
        try {
            OutputText = CommonEncoding.UTF8Decode(InputText);
        } catch (Exception error) {
            Logger.Error(error);
            MessageBox.Error("解码失败");
        }
    }
}

