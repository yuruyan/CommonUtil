using CommonUITools.Model;
using CommonUtil.Core;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class CodeColorizationView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty LanguagesProperty = DependencyProperty.Register("Languages", typeof(ExtendedObservableCollection<string>), typeof(CodeColorizationView), new PropertyMetadata());
    /// <summary>
    /// 语言
    /// </summary>
    public ExtendedObservableCollection<string> Languages {
        get { return (ExtendedObservableCollection<string>)GetValue(LanguagesProperty); }
        set { SetValue(LanguagesProperty, value); }
    }

    public CodeColorizationView() {
        Languages = new();
        // 后台加载
        Task.Run(() => {
            var languages = CodeColorization.Languages.ToArray();
            Array.Sort(languages);
            Dispatcher.Invoke(() => {
                Languages.AddRange(languages);
            });
        });
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
