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
using System.Windows.Input;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class JsonExtractorView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(JsonExtractorView), new PropertyMetadata(""));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(JsonExtractorView), new PropertyMetadata(""));
    public static readonly DependencyProperty PatternTextProperty = DependencyProperty.Register("PatternText", typeof(string), typeof(JsonExtractorView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(JsonExtractorView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(JsonExtractorView), new PropertyMetadata(false));
    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "Text|*.txt|All Files|*.*"
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
    /// 提取模式
    /// </summary>
    public string PatternText {
        get { return (string)GetValue(PatternTextProperty); }
        set { SetValue(PatternTextProperty, value); }
    }

    public JsonExtractorView() {
        InitializeComponent();
    }

    /// <summary>
    /// 处理数据提取
    /// </summary>
    private async void HandleExtract() {
        // 二进制文件警告
        if (HasFile && CommonUtils.IsLikelyBinaryFile(FileName)) {
            WarningDialog dialog = WarningDialog.Shared;
            dialog.DetailText = "文件可能是二进制文件，是否继续？";
            if (await dialog.ShowAsync() != ModernWpf.Controls.ContentDialogResult.Primary) {
                return;
            }
        }
        // 检查输入
        if (!HasFile) {
            if (!UIUtils.CheckInputNullOrEmpty(new KeyValuePair<string?, string>[] {
                new (InputText, "请输入文本"),
                new (PatternText, "请输入提取模式"),
            })) {
                return;
            }
        }

        // 数据提取
        if (!HasFile) {
            StringExtract();
            return;
        }
        ThrottleUtils.ThrottleAsync(HandleExtract, FileExtract);
    }

    /// <summary>
    /// 文本提取
    /// </summary>
    private void StringExtract() {
        try {
            OutputText = string.Concat(JsonExtractor.Extract(InputText, PatternText));
        } catch {
            MessageBox.Error("失败");
        }
    }

    /// <summary>
    /// 文件文本提取
    /// </summary>
    private async Task FileExtract() {
        var pattern = PatternText;
        var inputPath = FileName;
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var outputPath = SaveFileDialog.FileName;

        await Task.Run(() => {
            try {
                JsonExtractor.FileExtract(inputPath, outputPath, pattern);
                // 通知
                UIUtils.NotificationOpenFileInDirectoryAsync(outputPath);
            } catch (IOException) {
                MessageBox.Error("文件读取或写入失败");
            } catch (Exception e) {
                Console.WriteLine(e);
                MessageBox.Error("失败");
            }
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
        FileName = InputText = OutputText = PatternText = string.Empty;
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
    /// 按下回车提取
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PatternTextBoxKeyUpHandler(object sender, KeyEventArgs e) {
        e.Handled = true;
        if (e.Key == Key.Enter) {
            HandleExtract();
        }
    }

    /// <summary>
    /// 提取数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void JsonExtractClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        HandleExtract();
    }
}
