using CommonUtil.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class CodeColorizationView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public IList<string> Languages { get; }

    public CodeColorizationView() {
        var languages = CodeColorization.Languages.ToArray();
        Array.Sort(languages);
        Languages = languages;
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
        TextEditor.SelectAll();
        TextEditor.Copy();
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
}
