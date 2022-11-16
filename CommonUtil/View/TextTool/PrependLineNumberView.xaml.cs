﻿using CommonUITools.Utils;
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

public partial class PrependLineNumberView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(PrependLineNumberView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(PrependLineNumberView), new PropertyMetadata(""));
    public static readonly DependencyProperty SplitTextOptionsProperty = DependencyProperty.Register("SplitTextOptions", typeof(List<string>), typeof(PrependLineNumberView), new PropertyMetadata());
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(PrependLineNumberView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(PrependLineNumberView), new PropertyMetadata(false));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(PrependLineNumberView), new PropertyMetadata(true));
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
    /// 分隔文本选项
    /// </summary>
    public List<string> SplitTextOptions {
        get { return (List<string>)GetValue(SplitTextOptionsProperty); }
        set { SetValue(SplitTextOptionsProperty, value); }
    }
    /// <summary>
    /// 是否扩宽
    /// </summary>
    public bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }
    private readonly IDictionary<string, string> SplitTextDict;

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
    /// 增加行号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void PrependLineNumberClick(object sender, RoutedEventArgs e) {
        string separator = GetComboBoxText();
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }

        // 文本处理
        if (!HasFile) {
            StringPrependLineNumber(separator);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            PrependLineNumberClick,
            () => FilePrependLineNumber(separator)
        );
    }

    /// <summary>
    /// 文本增加行号
    /// </summary>
    /// <param name="separator">分隔符</param>
    private void StringPrependLineNumber(string separator) {
        OutputText = TextTool.PrependLineNumber(InputText, separator);
    }

    /// <summary>
    /// 文件文本增加行号
    /// </summary>
    /// <param name="separator">分隔符</param>
    private async Task FilePrependLineNumber(string separator) {
        var text = InputText;
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await UIUtils.CreateFileProcessTask(
            TextTool.FilePrependLineNumber,
            outputPath,
            args: new object[] { inputPath, outputPath, separator }
        );
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
