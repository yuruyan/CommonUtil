using CommonUITools.Utils;
using CommonUtil.Core;
using CommonUtil.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static QRCoder.PayloadGenerator;

namespace CommonUtil.View;

public partial class WIFIQRCodeView : Page, IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty WiFiNameProperty = DependencyProperty.Register("WiFiName", typeof(string), typeof(WIFIQRCodeView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(WIFIQRCodeView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IsHiddenWifiProperty = DependencyProperty.Register("IsHiddenWifi", typeof(bool), typeof(WIFIQRCodeView), new PropertyMetadata(false));

    /// <summary>
    /// wifi 名称
    /// </summary>
    public string WiFiName {
        get { return (string)GetValue(WiFiNameProperty); }
        set { SetValue(WiFiNameProperty, value); }
    }
    /// <summary>
    /// wifi 密码
    /// </summary>
    public string Password {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }
    /// <summary>
    /// 是否是隐藏 wifi
    /// </summary>
    public bool IsHiddenWifi {
        get { return (bool)GetValue(IsHiddenWifiProperty); }
        set { SetValue(IsHiddenWifiProperty, value); }
    }

    public WIFIQRCodeView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成二维码
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    Task<byte[]> IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>>.Generate(KeyValuePair<QRCodeFormat, QRCodeInfo> arg) {
        var wifiName = WiFiName ?? string.Empty;
        var password = Password ?? string.Empty;
        var isHiddenWifi = IsHiddenWifi;
        // 输入无效
        if (!(UIUtils.CheckInputNullOrEmpty(wifiName, message: "wifi 名称不能为空")
            && UIUtils.CheckInputNullOrEmpty(password, message: "wifi 密码不能为空")
        )) {
            return Task.FromResult(Array.Empty<byte>());
        }
        var authentication = WiFi.Authentication.WPA;
        // 设置加密方式
        if (AuthenticationComboBox.SelectedValue is ComboBoxItem item && item.Content is string method) {
            if (!method.ToLower().Contains("wpa")) {
                authentication = WiFi.Authentication.WEP;
            }
        }
        return Task.Run(() => QRCodeTool.GenerateQRCodeForWiFi(
            wifiName,
            password,
            arg.Value,
            authentication: authentication,
            isWifiHidden: isHiddenWifi,
            format: arg.Key
        ));
    }
}
