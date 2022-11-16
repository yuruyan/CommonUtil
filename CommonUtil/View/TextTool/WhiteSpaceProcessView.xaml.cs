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

public partial class WhiteSpaceProcessView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(WhiteSpaceProcessView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(WhiteSpaceProcessView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(WhiteSpaceProcessView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(WhiteSpaceProcessView), new PropertyMetadata(false));
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

    public WhiteSpaceProcessView() {
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

        bool trimText = TrimTextMenuItem.IsChecked == true;
        bool removeWhiteSpace = RemoveWhiteSpaceLineMenuItem.IsChecked == true;
        bool trimLine = TrimLineMenuItem.IsChecked == true;
        bool replaceMultiWhiteSpace = ReplaceMultipleWhiteSpaceWithOneMenuItem.IsChecked == true;

        // 文本处理
        if (!HasFile) {
            TextProcessString(trimText, removeWhiteSpace, trimLine, replaceMultiWhiteSpace);
            return;
        }
        ThrottleUtils.ThrottleAsync(
            TextProcessClick,
            () => TextProcessFile(trimText, removeWhiteSpace, trimLine, replaceMultiWhiteSpace)
        );
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    /// <param name="trimText"></param>
    /// <param name="removeWhiteSpace"></param>
    /// <param name="trimLine"></param>
    /// <param name="replaceMultiWhiteSpace"></param>
    private void TextProcessString(bool trimText, bool removeWhiteSpace, bool trimLine, bool replaceMultiWhiteSpace) {
        var s = InputText;
        if (trimText) {
            s = TextTool.TrimText(s);
        }
        if (removeWhiteSpace) {
            s = TextTool.RemoveWhiteSpaceLine(s);
        }
        if (trimLine) {
            s = TextTool.TrimLine(s);
        }
        if (replaceMultiWhiteSpace) {
            s = TextTool.ReplaceMultipleWhiteSpaceWithOne(s);
        }
        OutputText = s;
    }

    private async Task TextProcessFile(bool trimText, bool removeWhiteSpace, bool trimLine, bool replaceMultiWhiteSpace) {
        var text = InputText;
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await UIUtils.CreateFileProcessTask(
            () => {
                if (trimText) {
                    TextTool.FileTrimText(inputPath, outputPath);
                }
                if (removeWhiteSpace) {
                    TextTool.FileRemoveWhiteSpaceLine(inputPath, outputPath);
                }
                if (trimLine) {
                    TextTool.FileTrimLine(inputPath, outputPath);
                }
                if (replaceMultiWhiteSpace) {
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

}
