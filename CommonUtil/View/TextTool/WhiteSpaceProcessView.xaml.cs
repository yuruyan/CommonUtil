using CommonUtil.Core;
using NLog;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class WhiteSpaceProcessView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(WhiteSpaceProcessView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(WhiteSpaceProcessView), new PropertyMetadata(""));

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

    public WhiteSpaceProcessView() {
        InitializeComponent();
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextProcessClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var s = InputText;
        if (TrimTextCheckBox.IsChecked == true) {
            s = TextTool.TrimText(s);
        }
        if (RemoveWhiteSpaceLineCheckBox.IsChecked == true) {
            s = TextTool.RemoveWhiteSpaceLine(s);
        }
        if (TrimLineCheckBox.IsChecked == true) {
            s = TextTool.TrimLine(s);
        }
        if (ReplaceMultipleWhiteSpaceWithOneCheckBox.IsChecked == true) {
            s = TextTool.ReplaceMultipleWhiteSpaceWithOne(s);
        }
        OutputText = s;
    }

    /// <summary>
    /// 检查输入合法性
    /// </summary>
    /// <returns></returns>
    private bool CheckInputValidation() {
        bool r = InputText.Trim().Any();
        if (!r) {
            CommonUITools.Widget.MessageBox.Info("请输入文本");
        }
        return r;
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

