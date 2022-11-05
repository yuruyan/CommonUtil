using CommonUITools.Utils;
using CommonUtil.Core;
using ModernWpf.Controls;
using NLog;
using System;
using System.Linq;
using System.Windows;

namespace CommonUtil.View;

public partial class OrdinalTextGeneratorView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(OrdinalTextGeneratorView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(OrdinalTextGeneratorView), new PropertyMetadata(""));
    public static readonly DependencyProperty StartIndexProperty = DependencyProperty.Register("StartIndex", typeof(double), typeof(OrdinalTextGeneratorView), new PropertyMetadata(1.0));
    public static readonly DependencyProperty GenerationCountProperty = DependencyProperty.Register("GenerationCount", typeof(double), typeof(OrdinalTextGeneratorView), new PropertyMetadata(10.0));
    public static readonly DependencyProperty IsAscendantProperty = DependencyProperty.Register("IsAscendant", typeof(bool), typeof(OrdinalTextGeneratorView), new PropertyMetadata(true));

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
    /// 起始位置
    /// </summary>
    public double StartIndex {
        get { return (double)GetValue(StartIndexProperty); }
        set { SetValue(StartIndexProperty, value); }
    }
    /// <summary>
    /// 生成数量
    /// </summary>
    public double GenerationCount {
        get { return (double)GetValue(GenerationCountProperty); }
        set { SetValue(GenerationCountProperty, value); }
    }
    /// <summary>
    /// 是否正序
    /// </summary>
    public bool IsAscendant {
        get { return (bool)GetValue(IsAscendantProperty); }
        set { SetValue(IsAscendantProperty, value); }
    }

    public OrdinalTextGeneratorView() {
        InitializeComponent();
        InputText = "abc{} {{ }}";
        GenerateText();
    }

    /// <summary>
    /// 生成文本
    /// </summary>
    private void GenerateText() {
        try {
            var data = OrdinalTextGenerator.Generate(InputText, (int)StartIndex, (uint)GenerationCount);
            OutputText = string.Join('\n', IsAscendant ? data : data.Reverse());
        } catch (FormatException) {
            CommonUITools.Widget.MessageBox.Error("格式错误");
        } catch {
            CommonUITools.Widget.MessageBox.Error("生成失败");
        }
    }

    /// <summary>
    /// 生成按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GenerateClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        GenerateText();
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
        InputText = OutputText = string.Empty;
    }
}
