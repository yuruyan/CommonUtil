using CommonUtil.Core;
using CommonUtil.Model;
using NLog;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
        var receiver = Receiver;
        var subject = Subject;
        var message = Message;
        return Task.Run(() => QRCodeTool.GenerateQRCodeForMail(
            receiver,
            subject,
            message,
            arg.Value,
            arg.Key
        ));
    }
}
