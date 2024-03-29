﻿namespace CommonUtil.View;

public partial class EncodingTemplate : ResponsiveUserControl {
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

    public EncodingTemplate() : base(ResponsiveMode.Variable) {
        InitializeComponent();
    }

    /// <summary>
    /// 编码按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EncodingTextClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (string.IsNullOrEmpty(InputText)) {
            MessageBoxUtils.Info("请输入文本");
            return;
        }
        EncodingClick?.Invoke();
    }

    /// <summary>
    /// 解码按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DecodingTextClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (string.IsNullOrEmpty(InputText)) {
            MessageBoxUtils.Info("请输入文本");
            return;
        }
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
        MessageBoxUtils.Success("已复制");
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
