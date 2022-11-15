using CommonUITools.Utils;
using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class InvertTextView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(InvertTextView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(InvertTextView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(InvertTextView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(InvertTextView), new PropertyMetadata(false));
    public static readonly DependencyProperty InversionModeDictProperty = DependencyProperty.Register("InversionModeDict", typeof(IDictionary<string, TextTool.InversionMode>), typeof(InvertTextView), new PropertyMetadata());
    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "文本文件|*.txt|All Files|*.*"
    };

    /// <summary>
    /// 翻转模式
    /// </summary>
    public IDictionary<string, TextTool.InversionMode> InversionModeDict {
        get { return (IDictionary<string, TextTool.InversionMode>)GetValue(InversionModeDictProperty); }
        set { SetValue(InversionModeDictProperty, value); }
    }
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

    public InvertTextView() {
        InversionModeDict = new Dictionary<string, TextTool.InversionMode>() {
            {"水平翻转", TextTool.InversionMode.Horizontal },
            {"垂直翻转", TextTool.InversionMode.Vertical},
            {"全部翻转", TextTool.InversionMode.Both },
        };
        InitializeComponent();
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
        var inversionMode = InversionModeDict[InversionModeComboBox.SelectedValue.ToString()!];

        // 文本处理
        if (!HasFile) {
            StringTextProcess(inversionMode);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            TextProcessClick,
            () => FileTextProcess(inversionMode)
        );
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    /// <param name="includeNumber"></param>
    private void StringTextProcess(TextTool.InversionMode mode) {
        OutputText = TextTool.InvertText(InputText, mode);
    }

    /// <summary>
    /// 文件文本处理
    /// </summary>
    /// <param name="includeNumber"></param>
    private async Task FileTextProcess(TextTool.InversionMode mode) {
        var text = InputText;
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await UIUtils.CreateFileProcessTask(
            TextTool.FileInvertText,
            outputPath,
            args: new object[] { inputPath, outputPath, mode }
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
}
