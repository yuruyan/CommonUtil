using CommonUtil.Core;
using NLog;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class CodeColorizationView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public IEnumerable<string> Languages { get; }

    public CodeColorizationView() {
        Languages = CodeColorization.Languages;
        InitializeComponent();
        TextEditor.Options.ConvertTabsToSpaces = true;
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(TextEditor.Text);
        CommonUITools.Widget.MessageBox.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        TextEditor.Text = string.Empty;
    }

    private void TextEditor_Click(object sender, RoutedEventArgs e) {
        e.Handled = true;
    }
}
