using CommonUtil.Core;
using CommonUtil.Model;
using NLog;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class PhoneNumberQRCodeView : Page, IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty PhoneNumberProperty = DependencyProperty.Register("PhoneNumber", typeof(string), typeof(PhoneNumberQRCodeView), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 收件人
    /// </summary>
    public string PhoneNumber {
        get { return (string)GetValue(PhoneNumberProperty); }
        set { SetValue(PhoneNumberProperty, value); }
    }

    public PhoneNumberQRCodeView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成二维码
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    Task<byte[]> IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>>.Generate(KeyValuePair<QRCodeFormat, QRCodeInfo> arg) {
        var phoneNumber = PhoneNumber;
        return Task.Run(() => QRCodeTool.GenerateQRCodeForPhonenumber(
            phoneNumber,
            arg.Value,
            arg.Key
        ));
    }
}
