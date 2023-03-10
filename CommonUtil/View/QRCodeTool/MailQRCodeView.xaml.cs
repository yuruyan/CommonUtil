namespace CommonUtil.View;

public partial class MailQRCodeView : Page, IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty ReceiverProperty = DependencyProperty.Register("Receiver", typeof(string), typeof(MailQRCodeView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MailQRCodeView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty SubjectProperty = DependencyProperty.Register("Subject", typeof(string), typeof(MailQRCodeView), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 收件人
    /// </summary>
    public string Receiver {
        get { return (string)GetValue(ReceiverProperty); }
        set { SetValue(ReceiverProperty, value); }
    }
    /// <summary>
    /// 主题
    /// </summary>
    public string Subject {
        get { return (string)GetValue(SubjectProperty); }
        set { SetValue(SubjectProperty, value); }
    }
    /// <summary>
    /// 短信内容
    /// </summary>
    public string Message {
        get { return (string)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }

    public MailQRCodeView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成二维码
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    Task<byte[]> IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>>.Generate(KeyValuePair<QRCodeFormat, QRCodeInfo> arg) {
        var receiver = Receiver ?? string.Empty;
        var subject = Subject ?? string.Empty;
        var message = Message ?? string.Empty;
        // 检验输入
        if (!(UIUtils.CheckInputNullOrEmpty(receiver, message: "收件人不能为空")
            && UIUtils.CheckInputNullOrEmpty(message, message: "内容不能为空")
        )) {
            return Task.FromResult(Array.Empty<byte>());
        }
        return Task.Run(() => QRCodeTool.GenerateQRCodeForMail(
            receiver,
            subject,
            message,
            arg.Value,
            arg.Key
        ));
    }
}
