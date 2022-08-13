using CommonUITools.Converter;
using CommonUITools.Utils;
using CommonUITools.View;
using CommonUtil.Core;
using Microsoft.Win32;
using ModernWpf.Controls;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class Base64ToolView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 文件长度大于此则警告
    /// </summary>
    private const int FileLengthWaringThreshold = 1024 * 256;
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(Base64ToolView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(Base64ToolView), new PropertyMetadata(""));
    private readonly FileSizeConverter FileSizeConverter;

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

    public Base64ToolView() {
        InitializeComponent();
        FileSizeConverter = TryFindResource("FileSizeConverter") as FileSizeConverter ?? new();
    }

    /// <summary>
    /// 解码文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DecodeFile(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var openFileDialog = new OpenFileDialog() {
            Title = "选择文件",
            Filter = "All Files|*.*"
        };
        if (openFileDialog.ShowDialog() != true) {
            return;
        }
        // 长度大于阈值警告
        if (new FileInfo(openFileDialog.FileName).Length >= FileLengthWaringThreshold) {
            object sizeString = FileSizeConverter.Convert(
                FileLengthWaringThreshold,
                typeof(int),
                0,
                System.Globalization.CultureInfo.CurrentUICulture
            );
            WarningDialog.Shared.DetailText = $"文件大于 {sizeString} ，可能造成卡顿，是否继续？";
            if (await WarningDialog.Shared.ShowAsync() != ContentDialogResult.Primary) {
                return;
            }
        }

        // 解码
        try {
            string result = await Task.Run(() => Base64Tool.Base64DecodeFile(openFileDialog.FileName));
            OutputText = result;
        } catch (IOException) {
            MessageBox.Error("文件读取失败");
        } catch (Exception) {
            MessageBox.Error("解码失败");
        }
    }

    /// <summary>
    /// 编码文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void EncodeFile(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var openFileDialog = new OpenFileDialog() {
            Title = "选择文件",
            Filter = "All Files|*.*"
        };
        if (openFileDialog.ShowDialog() != true) {
            return;
        }
        // 长度大于阈值警告
        if (new FileInfo(openFileDialog.FileName).Length >= FileLengthWaringThreshold) {
            object sizeString = FileSizeConverter.Convert(
                FileLengthWaringThreshold,
                typeof(int),
                0,
                System.Globalization.CultureInfo.CurrentUICulture
            );
            WarningDialog.Shared.DetailText = $"文件大于 {sizeString} ，可能造成卡顿，是否继续？";
            if (await WarningDialog.Shared.ShowAsync() != ContentDialogResult.Primary) {
                return;
            }
        }

        // 编码
        try {
            string result = await Task.Run(() => {
                return Base64Tool.Base64EncodeFile(openFileDialog.FileName);
            });
            OutputText = result;
        } catch (IOException) {
            MessageBox.Error("文件读取失败");
        } catch (Exception) {
            MessageBox.Error("编码失败");
        }
    }

    /// <summary>
    /// 字符串解码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DecodeString(object sender, RoutedEventArgs e) {
        e.Handled = true;
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
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EncodeString(object sender, RoutedEventArgs e) {
        e.Handled = true;
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
        OutputText = InputText = string.Empty;
    }
}
