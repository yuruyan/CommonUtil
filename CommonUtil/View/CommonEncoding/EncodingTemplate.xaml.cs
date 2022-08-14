using NLog;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class EncodingTemplate : UserControl {
    private static Logger Logger = LogManager.GetCurrentClassLogger();

    public event Action? EncodingClick;
    public event Action? DecodingClick;

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(EncodingTemplate), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(EncodingTemplate), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public static readonly DependencyProperty EncodingButtonTextProperty = DependencyProperty.Register("EncodingButtonText", typeof(string), typeof(EncodingTemplate), new PropertyMetadata(""));
    public static readonly DependencyProperty DecodingButtonTextProperty = DependencyProperty.Register("DecodingButtonText", typeof(string), typeof(EncodingTemplate), new PropertyMetadata(""));

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
    /// 编码按钮文本
    /// </summary>
    public string EncodingButtonText {
        get { return (string)GetValue(EncodingButtonTextProperty); }
        set { SetValue(EncodingButtonTextProperty, value); }
    }
    /// <summary>
    /// 解码按钮文本
    /// </summary>
    public string DecodingButtonText {
        get { return (string)GetValue(DecodingButtonTextProperty); }
        set { SetValue(DecodingButtonTextProperty, value); }
    }

    public EncodingTemplate() {
        InitializeComponent();
    }

    /// <summary>
    /// 编码按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EncodingTextClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        EncodingClick?.Invoke();
    }

    /// <summary>
    /// 解码按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DecodingTextClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        DecodingClick?.Invoke();
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        CommonUITools.Widget.MessageBox.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        InputText = string.Empty;
        OutputText = string.Empty;
    }

}

