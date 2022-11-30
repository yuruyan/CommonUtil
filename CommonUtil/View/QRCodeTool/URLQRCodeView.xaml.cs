using CommonUITools.Utils;
using CommonUtil.Core;
using CommonUtil.Core.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class URLQRCodeView : Page, IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty URLTextProperty = DependencyProperty.Register("URLText", typeof(string), typeof(URLQRCodeView), new PropertyMetadata(string.Empty));
    /// <summary>
    /// url
    /// </summary>
    public string URLText {
        get { return (string)GetValue(URLTextProperty); }
        set { SetValue(URLTextProperty, value); }
    }

    public URLQRCodeView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成二维码
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    Task<byte[]> IGenerable<KeyValuePair<QRCodeFormat, QRCodeInfo>, Task<byte[]>>.Generate(KeyValuePair<QRCodeFormat, QRCodeInfo> arg) {
        var url = URLText;
        // 检验输入
        if (!UIUtils.CheckInputNullOrEmpty(url, message: "链接不能为空")) {
            return Task.FromResult(Array.Empty<byte>());
        }
        return Task.Run(() => QRCodeTool.GenerateQRCodeForText(
            url,
            arg.Value,
            arg.Key
        ));
    }
}
