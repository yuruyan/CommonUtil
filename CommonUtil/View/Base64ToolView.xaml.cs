using CommonUITools.Utils;
using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class Base64ToolView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(Base64ToolView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(Base64ToolView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(Base64ToolView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(Base64ToolView), new PropertyMetadata(false));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(Base64ToolView), new PropertyMetadata(true));
    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "All Files|*.*"
    };

    /// <summary>
    /// 输入
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }
    /// <summary>
    /// 输出结果
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

    public Base64ToolView() {
        InitializeComponent();
        // 响应式布局
        UIUtils.SetLoadedOnceEventHandler(this, (_, _) => {
            Window window = Window.GetWindow(this);
            double expansionThreshold = (double)Resources["ExpansionThreshold"];
            DependencyPropertyDescriptor
                .FromProperty(Window.ActualWidthProperty, typeof(Window))
                .AddValueChanged(window, (_, _) => {
                    IsExpanded = window.ActualWidth >= expansionThreshold;
                });
        });
    }

    /// <summary>
    /// 解码文件
    /// </summary>
    private async Task DecodeFile() {
        var filename = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var savePath = SaveFileDialog.FileName;
        // 解码
        await Task.Run(async () => {
            try {
                var result = Base64Tool.Base64DecodeFile(filename);
                // 保存文件
                try {
                    await File.WriteAllBytesAsync(savePath, result);
                    // 通知
                    UIUtils.NotificationOpenFileInExplorerAsync(savePath, title: "解码文件成功");
                } catch {
                    MessageBox.Error("文件保存失败");
                }
            } catch (IOException) {
                MessageBox.Error("文件读取失败");
            } catch (Exception) {
                MessageBox.Error("解码失败");
            }
        });
    }

    /// <summary>
    /// 编码文件
    /// </summary>
    private async Task EncodeFile() {
        var filename = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var savePath = SaveFileDialog.FileName;
        // 编码
        await Task.Run(async () => {
            try {
                string result = Base64Tool.Base64EncodeFile(filename);
                // 保存文件
                try {
                    await File.WriteAllTextAsync(savePath, result);
                    // 通知
                    UIUtils.NotificationOpenFileInExplorerAsync(savePath, title: "编码文件成功");
                } catch {
                    MessageBox.Error("文件保存失败");
                }
            } catch (IOException) {
                MessageBox.Error("文件读取失败");
            } catch (Exception) {
                MessageBox.Error("编码失败");
            }
        });
    }

    /// <summary>
    /// 字符串解码
    /// </summary>
    private void DecodeString() {
        if (!UIUtils.CheckInputNullOrEmpty(InputText)) {
            return;
        }
        string? output = CommonUtils.Try(() => Base64Tool.Base64DecodeString(InputText));
        if (output is null) {
            MessageBox.Error("解码失败");
            return;
        }
        OutputText = output;
    }

    /// <summary>
    /// 字符串编码
    /// </summary>
    private void EncodeString() {
        if (!UIUtils.CheckInputNullOrEmpty(InputText)) {
            return;
        }
        string? output = CommonUtils.Try(() => Base64Tool.Base64EncodeString(InputText));
        if (output is null) {
            MessageBox.Error("编码失败");
            return;
        }
        OutputText = output;
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
        FileName = OutputText = InputText = string.Empty;
        DragDropTextBox.Clear();
    }

    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EncodeClickHandler(object sender, RoutedEventArgs e) {
        // 输入检查
        if (!HasFile && string.IsNullOrEmpty(InputText)) {
            MessageBox.Info("请输入文本");
            return;
        }

        // 处理
        if (!HasFile) {
            EncodeString();
            return;
        }
        ThrottleUtils.ThrottleAsync(EncodeClickHandler, EncodeFile);
    }

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DecodeClickHandler(object sender, RoutedEventArgs e) {
        // 输入检查
        if (!await UIUtils.CheckTextAndFileInputAsync(InputText, HasFile, FileName)) {
            return;
        }

        // 处理
        if (!HasFile) {
            DecodeString();
            return;
        }
        ThrottleUtils.ThrottleAsync(DecodeClickHandler, DecodeFile);
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
