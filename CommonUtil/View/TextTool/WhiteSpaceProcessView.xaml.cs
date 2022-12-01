using CommonUITools.Utils;
using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class WhiteSpaceProcessView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(WhiteSpaceProcessView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(WhiteSpaceProcessView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(WhiteSpaceProcessView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(WhiteSpaceProcessView), new PropertyMetadata(false));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(WhiteSpaceProcessView), new PropertyMetadata(true));
    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "文本文件|*.txt|All Files|*.*"
    };

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
    /// 是否有文件
    /// </summary>
    public bool HasFile {
        get { return (bool)GetValue(HasFileProperty); }
        set { SetValue(HasFileProperty, value); }
    }
    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName {
        get { return (string)GetValue(FileNameProperty); }
        set { SetValue(FileNameProperty, value); }
    }
    /// <summary>
    /// 是否扩宽
    /// </summary>
    public bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }

    public WhiteSpaceProcessView() {
        InitializeComponent();
        // 响应式布局
        UIUtils.SetLoadedOnceEventHandler(this, (_, _) => {
            Window window = Window.GetWindow(this);
            double expansionThreshold = (double)Resources["ExpansionThreshold"];
            IsExpanded = window.ActualWidth >= expansionThreshold;
            DependencyPropertyDescriptor
                .FromProperty(Window.ActualWidthProperty, typeof(Window))
                .AddValueChanged(window, (_, _) => {
                    IsExpanded = window.ActualWidth >= expansionThreshold;
                });
        });
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void TextProcessClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }
        var options = new ProcessOptions {
            TrimText = TrimTextMenuItem.IsChecked,
            TrimLine = TrimLineMenuItem.IsChecked,
            TrimLineStart = TrimLineStartMenuItem.IsChecked,
            TrimLineEnd = TrimLineEndMenuItem.IsChecked,
            RemoveWhiteSpace = RemoveWhiteSpaceLineMenuItem.IsChecked,
            ReplaceMultiWhiteSpace = ReplaceMultipleWhiteSpaceWithOneMenuItem.IsChecked,
        };

        // 文本处理
        if (!HasFile) {
            TextProcessString(options);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            TextProcessClick,
            () => TextProcessFile(options)
        );
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    /// <param name="options">选项</param>
    private void TextProcessString(ProcessOptions options) {
        var s = InputText;
        if (options.TrimText) {
            s = TextTool.TrimText(s);
        }
        if (options.RemoveWhiteSpace) {
            s = TextTool.RemoveWhiteSpaceLine(s);
        }
        if (options.TrimLineStart) {
            s = TextTool.TrimLineStart(s);
        }
        if (options.TrimLineEnd) {
            s = TextTool.TrimLineEnd(s);
        }
        if (options.TrimLine) {
            s = TextTool.TrimLine(s);
        }
        if (options.ReplaceMultiWhiteSpace) {
            s = TextTool.ReplaceMultipleWhiteSpaceWithOne(s);
        }
        OutputText = s;
    }

    /// <summary>
    /// 文件处理
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private async Task TextProcessFile(ProcessOptions options) {
        var text = InputText;
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await UIUtils.CreateFileProcessTask(
            () => {
                if (options.TrimText) {
                    TextTool.FileTrimText(inputPath, outputPath);
                }
                if (options.RemoveWhiteSpace) {
                    TextTool.FileRemoveWhiteSpaceLine(inputPath, outputPath);
                }
                if (options.TrimLine) {
                    TextTool.FileTrimLine(inputPath, outputPath);
                }
                if (options.ReplaceMultiWhiteSpace) {
                    TextTool.FileReplaceMultipleWhiteSpaceWithOne(inputPath, outputPath);
                }
            },
            outputPath
        );
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        MessageBox.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        InputText = OutputText = string.Empty;
        DragDropTextBox.Clear();
    }

    /// <summary>
    /// DragDropEvent
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DragDropEventHandler(object sender, object e) {
        if (e is IEnumerable<string> array) {
            if (!array.Any()) {
                return;
            }
            // 判断是否为文件
            if (File.Exists(array.First())) {
                FileName = array.First();
            } else {
                DragDropTextBox.Clear();
            }
        }
    }

    /// <summary>
    /// 处理选项
    /// </summary>
    private readonly struct ProcessOptions {
        public bool TrimText { get; init; }
        public bool RemoveWhiteSpace { get; init; }
        public bool TrimLine { get; init; }
        public bool TrimLineStart { get; init; }
        public bool TrimLineEnd { get; init; }
        public bool ReplaceMultiWhiteSpace { get; init; }
    }
}
