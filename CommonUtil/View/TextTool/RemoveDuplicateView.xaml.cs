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

public partial class RemoveDuplicateView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RemoveDuplicateView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(RemoveDuplicateView), new PropertyMetadata(""));
    public static readonly DependencyProperty SymbolOptionsProperty = DependencyProperty.Register("SymbolOptions", typeof(List<string>), typeof(RemoveDuplicateView), new PropertyMetadata());
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(RemoveDuplicateView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(RemoveDuplicateView), new PropertyMetadata(false));
    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "All Files|*.*"
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
    /// 标记选项
    /// </summary>
    public List<string> SymbolOptions {
        get { return (List<string>)GetValue(SymbolOptionsProperty); }
        set { SetValue(SymbolOptionsProperty, value); }
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
    private Dictionary<string, string> SymbolDict;

    public RemoveDuplicateView() {
        SymbolDict = new() {
            { "换行符（⮠  ）", "\n" },
            { "制表符（→）", "\t" },
            { "空格（ ）", " " },
            { "中文逗号（，）", "，" },
            { "英文逗号（,）", "," },
        };
        SymbolOptions = new(SymbolDict.Keys);
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
    /// 字符串去重
    /// </summary>
    /// <param name="splitSymbol"></param>
    /// <param name="mergeSymbol"></param>
    /// <param name="trim"></param>
    private void StringRemoveDuplicate(string splitSymbol, string mergeSymbol, bool trim) {
        OutputText = TextTool.RemoveDuplicate(InputText, splitSymbol, mergeSymbol, trim);
    }

    /// <summary>
    /// 文件文本去重
    /// </summary>
    /// <param name="splitSymbol"></param>
    /// <param name="mergeSymbol"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    private async Task FileRemoveDuplicate(string splitSymbol, string mergeSymbol, bool trim) {
        var text = InputText;
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        await Task.Run(() => {
            try {
                TextTool.FileRemoveDuplicate(inputPath, outputPath, splitSymbol, mergeSymbol, trim);
                // 通知
                UIUtils.NotificationOpenFileInDirectoryAsync(outputPath);
            } catch (IOException) {
                MessageBox.Error("文件读取或写入失败");
            } catch {
                MessageBox.Error("失败");
            }
        });
    }

    /// <summary>
    /// 文本去重
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RemoveDuplicateClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (!HasFile && InputText == string.Empty) {
            MessageBox.Info("请输入文本");
            return;
        }
        var splitSymbol = GetComboBoxText(SplitSymbolBox);
        var mergeSymbol = GetComboBoxText(MergeSymbolBox);
        var trim = TrimWhiteSpaceCheckBox.IsChecked == true;

        if (!HasFile) {
            StringRemoveDuplicate(splitSymbol, mergeSymbol, trim);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            RemoveDuplicateClick,
            () => FileRemoveDuplicate(splitSymbol, mergeSymbol, trim)
        );
    }

    /// <summary>
    /// 获取 ComboBox 文本
    /// </summary>
    /// <param name="comboBox"></param>
    /// <returns></returns>
    private string GetComboBoxText(ComboBox comboBox) {
        object selectedValue = comboBox.SelectedValue;
        string text = comboBox.Text;
        // 非用户输入
        if (selectedValue != null) {
            if (selectedValue is string t) {
                text = SymbolDict[t];
            }
        }
        return text;
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
