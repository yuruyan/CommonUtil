using CommonUITools.Utils;
using CommonUITools.View;
using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class ChineseTransformView : Page {
    private static Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(ChineseTransformView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(ChineseTransformView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(ChineseTransformView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(ChineseTransformView), new PropertyMetadata(false));
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

    public ChineseTransformView() {
        InitializeComponent();
    }

    /// <summary>
    /// 文字转简体
    /// </summary>
    private void StringToSimplified() {
        OutputText = ChineseTransform.ToSimplified(InputText);
    }

    /// <summary>
    /// 文件转繁体
    /// </summary>
    private void StringToTraditional() {
        OutputText = ChineseTransform.ToTraditional(InputText);
    }

    /// <summary>
    /// 文件转简体
    /// </summary>
    private async Task FileToSimplified() {
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await UIUtils.CreateFileProcessTask(
            ChineseTransform.FileToSimplified,
            outputPath,
            args: new object[] { inputPath, outputPath }
        );
    }

    /// <summary>
    /// 文件转繁体
    /// </summary>
    private async Task FileToTraditional() {
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        await Task.Run(() => {
            try {
                ChineseTransform.FileToTraditional(inputPath, outputPath);
                // 通知
                CommonUITools.Widget.NotificationBox.Success(
                    "转换成功",
                    "点击打开",
                    () => UIUtils.OpenFileInDirectoryAsync(outputPath)
                );
            } catch (IOException) {
                MessageBox.Error("文件读取或写入失败");
            } catch {
                MessageBox.Error("转换失败");
            }
        });
    }

    /// <summary>
    /// 转简体
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ToSimplifiedClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }

        if (!HasFile) {
            StringToSimplified();
            return;
        }
        ThrottleUtils.ThrottleAsync(ToSimplifiedClick, FileToSimplified);
    }

    /// <summary>
    /// 转繁体
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ToTraditionalClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }

        if (!HasFile) {
            StringToTraditional();
            return;
        }
        ThrottleUtils.ThrottleAsync(ToTraditionalClick, FileToTraditional);
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
        FileName = InputText = OutputText = string.Empty;
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
