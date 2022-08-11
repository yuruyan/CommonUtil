using CommonUtil.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class PrependLineNumberView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(PrependLineNumberView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(PrependLineNumberView), new PropertyMetadata(""));
    public static readonly DependencyProperty SplitTextOptionsProperty = DependencyProperty.Register("SplitTextOptions", typeof(List<string>), typeof(PrependLineNumberView), new PropertyMetadata());

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
    /// 分隔文本选项
    /// </summary>
    public List<string> SplitTextOptions {
        get { return (List<string>)GetValue(SplitTextOptionsProperty); }
        set { SetValue(SplitTextOptionsProperty, value); }
    }
    private IDictionary<string, string> SplitTextDict;

    public PrependLineNumberView() {
        SplitTextDict = new Dictionary<string, string>() {
            { "制表符（→）", "\t" },
            { "空格（ ）", " " },
            { "中文逗号（，）", "，" },
            { "英文逗号（,）", "," },
            { "中文句号（。）", "。" },
            { "英文句号（.）", "." },
        };
        SplitTextOptions = new(SplitTextDict.Keys);
        InitializeComponent();
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

    /// <summary>
    /// 增加行号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PrependLineNumberClick(object sender, RoutedEventArgs e) {
        string splitText = GetComboBoxText();
        Console.WriteLine("splitText: " + splitText);
        OutputText = TextTool.PrependLineNumber(InputText, splitText);
    }

    /// <summary>
    /// 获取 ComboBox 文本
    /// </summary>
    /// <returns></returns>
    private string GetComboBoxText() {
        object selectedValue = SplitTextComboBox.SelectedValue;
        string text = SplitTextComboBox.Text;
        // 非用户输入
        if (selectedValue != null) {
            if (selectedValue is string t) {
                text = SplitTextDict[t];
            }
        }
        return text;
    }
}

