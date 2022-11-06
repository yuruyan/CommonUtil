using CommonUITools.Utils;
using CommonUITools.View;
using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class HalfFullCharTransformView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(HalfFullCharTransformView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(HalfFullCharTransformView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(HalfFullCharTransformView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(HalfFullCharTransformView), new PropertyMetadata(false));
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

    public HalfFullCharTransformView() {
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
    /// 半角转全角
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void HalfToFullCharClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 二进制文件警告
        if (HasFile && CommonUtils.IsLikelyBinaryFile(FileName)) {
            WarningDialog dialog = WarningDialog.Shared;
            dialog.DetailText = "文件可能是二进制文件，是否继续？";
            if (await dialog.ShowAsync() != ModernWpf.Controls.ContentDialogResult.Primary) {
                return;
            }
        }
        // 输入检查
        if (!HasFile && InputText.Length == 0) {
            MessageBox.Info("请输入文本");
            return;
        }

        // 文本处理
        if (!HasFile) {
            StringHalfToFullChar();
            return;
        }
        ThrottleUtils.ThrottleAsync(HalfToFullCharClick, FileHalfToFullChar);
    }

    /// <summary>
    /// 全角转半角
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void FullToHalfCharClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 二进制文件警告
        if (HasFile && CommonUtils.IsLikelyBinaryFile(FileName)) {
            WarningDialog dialog = WarningDialog.Shared;
            dialog.DetailText = "文件可能是二进制文件，是否继续？";
            if (await dialog.ShowAsync() != ModernWpf.Controls.ContentDialogResult.Primary) {
                return;
            }
        }
        // 输入检查
        if (!HasFile && InputText.Length == 0) {
            MessageBox.Info("请输入文本");
            return;
        }

        // 文本处理
        if (!HasFile) {
            StringFullToHalfChar();
            return;
        }
        ThrottleUtils.ThrottleAsync(FullToHalfCharClick, FileFullToHalfChar);
    }

    /// <summary>
    /// 文本半角转全角
    /// </summary>
    private void StringHalfToFullChar() {
        OutputText = TextTool.HalfCharToFullChar(InputText);
    }

    /// <summary>
    /// 文本全角转半角
    /// </summary>
    private void StringFullToHalfChar() {
        OutputText = TextTool.FullCharToHalfChar(InputText);
    }

    /// <summary>
    /// 文件文本半角转全角
    /// </summary>
    private async Task FileHalfToFullChar() {
        var text = InputText;
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await Task.Run(() => {
            try {
                TextTool.FileHalfCharToFullChar(inputPath, outputPath);
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
    /// 文件文本半角转全角
    /// </summary>
    private async Task FileFullToHalfChar() {
        var text = InputText;
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        // 处理
        await Task.Run(() => {
            try {
                TextTool.FileFullCharToHalfChar(inputPath, outputPath);
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
