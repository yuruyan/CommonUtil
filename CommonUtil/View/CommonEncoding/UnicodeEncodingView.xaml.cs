﻿using UnicodeEncoding = CommonUtil.Core.UnicodeEncoding;

namespace CommonUtil.View;

public partial class UnicodeEncodingView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(UnicodeEncodingView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(UnicodeEncodingView), new PropertyMetadata(""));

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

    public UnicodeEncodingView() {
        InitializeComponent();
    }

    /// <summary>
    /// 编码
    /// </summary>
    private void EncodingClick() {
        try {
            OutputText = UnicodeEncoding.UnicodeEncode(InputText);
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
            OutputText = UnicodeEncoding.UnicodeDecode(InputText);
        } catch (Exception error) {
            Logger.Error(error);
            MessageBoxUtils.Error("解码失败");
        }
    }
}

