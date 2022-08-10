using CommonUITools.Utils;
using CommonUtil.Core;
using CommonUtil.Store;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace CommonUtil.View;

internal class ProcessVisibilityConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        short v = System.Convert.ToInt16(value);
        if (v == 0 || v == 100) {
            return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

public partial class DataDigestView : Page {

    private class DigestInfo : DependencyObject {
        public static readonly DependencyProperty IsVivibleProperty = DependencyProperty.Register("IsVivible", typeof(bool), typeof(DigestInfo), new PropertyMetadata(false));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DigestInfo), new PropertyMetadata(""));
        public static readonly DependencyProperty ProcessProperty = DependencyProperty.Register("Process", typeof(int), typeof(DigestInfo), new PropertyMetadata(0));

        public DigestInfo(TextDigestHandler textDigestHandler, StreamDigestHandler streamDigestHandler) {
            TextDigestHandler = textDigestHandler;
            StreamDigestHandler = streamDigestHandler;
        }

        /// <summary>
        /// 文本 Hash 处理器
        /// </summary>
        public TextDigestHandler TextDigestHandler { get; set; }
        /// <summary>
        /// 文件流 Hash 处理器
        /// </summary>
        public StreamDigestHandler StreamDigestHandler { get; set; }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVivible {
            get { return (bool)GetValue(IsVivibleProperty); }
            set { SetValue(IsVivibleProperty, value); }
        }
        /// <summary>
        /// 结果
        /// </summary>
        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// 进度
        /// </summary>
        public int Process {
            get { return (int)GetValue(ProcessProperty); }
            set { SetValue(ProcessProperty, value); }
        }
        /// <summary>
        /// 上次更新 Process 时间
        /// </summary>
        public DateTime LastUpdateProcessTime { get; set; } = DateTime.Now;
    }
    /// <summary>
    /// 更新 Process 频率，ms
    /// </summary>
    private const int UpdateProcessFrequency = 500;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private delegate string TextDigestHandler(string digest);
    /// <summary>
    /// StreamDigestHandler
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="callback">回调，参数为总读取的大小</param>
    /// <returns></returns>
    private delegate string StreamDigestHandler(FileStream stream, Action<long>? callback = null);

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(DataDigestView), new PropertyMetadata(""));
    public static readonly DependencyProperty DigestOptionsProperty = DependencyProperty.Register("DigestOptions", typeof(List<string>), typeof(DataDigestView), new PropertyMetadata());
    public static readonly DependencyProperty SelectedDigestIndexProperty = DependencyProperty.Register("SelectedDigestIndex", typeof(int), typeof(DataDigestView), new PropertyMetadata(0));
    private static readonly DependencyProperty DigestInfoDictProperty = DependencyProperty.Register("DigestInfoDict", typeof(Dictionary<string, DigestInfo>), typeof(DataDigestView), new PropertyMetadata());
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(DataDigestView), new PropertyMetadata(""));
    public static readonly DependencyProperty RunningProcessProperty = DependencyProperty.Register("RunningProcess", typeof(int), typeof(DataDigestView), new PropertyMetadata(0));
    public static readonly DependencyProperty FileIconProperty = DependencyProperty.Register("FileIcon", typeof(string), typeof(DataDigestView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileSizeProperty = DependencyProperty.Register("FileSize", typeof(long), typeof(DataDigestView), new PropertyMetadata(0L));
    public static readonly DependencyProperty IsStopButtonVisibleProperty = DependencyProperty.Register("IsStopButtonVisible", typeof(bool), typeof(DataDigestView), new PropertyMetadata(false));

    /// <summary>
    /// 输入文件名
    /// </summary>
    public string FileName {
        get { return (string)GetValue(FileNameProperty); }
        set { SetValue(FileNameProperty, value); }
    }
    /// <summary>
    /// 输入文本
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }
    /// <summary>
    /// 散列算法选择
    /// </summary>
    public List<string> DigestOptions {
        get { return (List<string>)GetValue(DigestOptionsProperty); }
        set { SetValue(DigestOptionsProperty, value); }
    }
    /// <summary>
    /// 选中的 Digest 算法
    /// </summary>
    public int SelectedDigestIndex {
        get { return (int)GetValue(SelectedDigestIndexProperty); }
        set { SetValue(SelectedDigestIndexProperty, value); }
    }
    /// <summary>
    /// 摘要算法 Dict
    /// </summary>
    private Dictionary<string, DigestInfo> DigestInfoDict {
        get { return (Dictionary<string, DigestInfo>)GetValue(DigestInfoDictProperty); }
        set { SetValue(DigestInfoDictProperty, value); }
    }
    /// <summary>
    /// 当前进行的任务
    /// </summary>
    public int RunningProcess {
        get { return (int)GetValue(RunningProcessProperty); }
        set { SetValue(RunningProcessProperty, value); }
    }
    /// <summary>
    /// 文件图标
    /// </summary>
    public string FileIcon {
        get { return (string)GetValue(FileIconProperty); }
        set { SetValue(FileIconProperty, value); }
    }
    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize {
        get { return (long)GetValue(FileSizeProperty); }
        set { SetValue(FileSizeProperty, value); }
    }
    /// <summary>
    /// 停止按钮是否可见
    /// </summary>
    public bool IsStopButtonVisible {
        get { return (bool)GetValue(IsStopButtonVisibleProperty); }
        set { SetValue(IsStopButtonVisibleProperty, value); }
    }
    /// <summary>
    /// 正在工作的 stream
    /// </summary>
    private readonly ICollection<FileStream> WorkingDigestStream = new List<FileStream>();
    public DataDigestView() {
        DigestOptions = new() {
                "全部",
                "MD2",
                "MD4",
                "MD5",
                "SHA1",
                "SHA3",
                "SHA224",
                "SHA256",
                "SHA384",
                "SHA512",
            };
        DigestInfoDict = new() {
            { "MD2", new(DataDigest.MD2Digest, DataDigest.MD2Digest) },
            { "MD4", new(DataDigest.MD4Digest, DataDigest.MD4Digest) },
            { "MD5", new(DataDigest.MD5Digest, DataDigest.MD5Digest) },
            { "SHA1", new(DataDigest.SHA1Digest, DataDigest.SHA1Digest) },
            { "SHA3", new(DataDigest.SHA3Digest, DataDigest.SHA3Digest) },
            { "SHA224", new(DataDigest.SHA224Digest, DataDigest.SHA224Digest) },
            { "SHA256", new(DataDigest.SHA256Digest, DataDigest.SHA256Digest) },
            { "SHA384", new(DataDigest.SHA384Digest, DataDigest.SHA384Digest) },
            { "SHA512", new(DataDigest.SHA512Digest, DataDigest.SHA512Digest) },
        };
        InitializeComponent();
        DependencyPropertyDescriptor.FromProperty(FileNameProperty, typeof(DataDigestView)).AddValueChanged(this, FileNameChangedHandler);
    }

    private void FileNameChangedHandler(object? sender, EventArgs e) {
        if (string.IsNullOrEmpty(FileName)) {
            ClearValue(FileIconProperty);
            return;
        }
        FileSize = new FileInfo(FileName).Length;
        FileIcon = FileIconUtils.GetIcon(FileName);
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        InputText = string.Empty;
        FileName = string.Empty;
        foreach (var item in DigestInfoDict) {
            item.Value.IsVivible = false;
        }
    }

    /// <summary>
    /// 开始计算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StartClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        ThrottleUtils.ThrottleAsync(StartClick, async () => {
            IsStopButtonVisible = true;
            await CalculateDigest();
        });
    }

    /// <summary>
    /// 计算 Hash
    /// </summary>
    /// <returns></returns>
    private async Task CalculateDigest() {
        if (SelectedDigestIndex != 0) {
            var target = DigestInfoDict[DigestOptions[SelectedDigestIndex]];
            // 隐藏其他
            foreach (var item in DigestInfoDict.Values) {
                if (item != target) {
                    item.IsVivible = false;
                }
            }
            await CalculateDigest(new DigestInfo[] { target });
            return;
        }
        // 全部计算
        await CalculateDigest(DigestInfoDict.Values);
    }

    /// <summary>
    /// 计算 Hash
    /// </summary>
    /// <param name="digests"></param>
    /// <returns></returns>
    private async Task CalculateDigest(IEnumerable<DigestInfo> digests) {
        // 先清空
        RunningProcess = 0;
        foreach (var item in digests) {
            item.Text = string.Empty;
            item.Process = 0;
            RunningProcess++;
        }
        var tasks = new List<Task>(RunningProcess);
        // 计算
        foreach (var item in digests) {
            item.IsVivible = true;
            var text = InputText;
            var filename = FileName;
            FileStream? fileStream = null;
            if (!string.IsNullOrEmpty(filename)) {
                fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                WorkingDigestStream.Add(fileStream);
            }
            tasks.Add(Task.Run(() => {
                string resultHash = string.Empty;
                // 计算文本 Hash
                if (fileStream is null) {
                    resultHash = item.TextDigestHandler.Invoke(text);
                } else {
                    // 计算文件 Hash
                    resultHash = CalculateFileDigest(item, fileStream);
                }
                // 更新
                Dispatcher.Invoke(() => {
                    item.Text = resultHash;
                    item.Process = 100;
                    RunningProcess--;
                });
                if (fileStream is not null) {
                    WorkingDigestStream.Remove(fileStream);
                    fileStream.Close();
                }
            }));
        }
        // 等待全部完成
        await Task.WhenAll(tasks);
        IsStopButtonVisible = false;
    }

    /// <summary>
    /// 计算文件摘要
    /// </summary>
    /// <param name="info"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    private string CalculateFileDigest(DigestInfo info, FileStream fileStream) {
        try {
            return info.StreamDigestHandler.Invoke(
                fileStream,
                read => {
                    // 控制更新频率
                    if ((DateTime.Now - info.LastUpdateProcessTime).TotalMilliseconds < UpdateProcessFrequency) {
                        return;
                    }
                    info.LastUpdateProcessTime = DateTime.Now;
                    Dispatcher.Invoke(() => {
                        info.Process = (int)(100 * (double)read / FileSize);
                    });
                }
            );
        } catch (Exception e) {
            CommonUITools.Widget.MessageBox.Error(e.Message);
            return string.Empty;
        }
    }

    /// <summary>
    /// 取消计算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StopDigestClick(object sender, RoutedEventArgs e) {
        foreach (var stream in WorkingDigestStream) {
            DataDigest.StopDigest(stream);
        }
        IsStopButtonVisible = false;
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var sb = new StringBuilder();
        foreach (var item in DigestInfoDict) {
            if (item.Value.IsVivible) {
                sb.Append($"{item.Key}: {item.Value.Text}\n");
            }
        }
        Clipboard.SetDataObject(sb.ToString());
        CommonUITools.Widget.MessageBox.Success("已复制");
    }

    /// <summary>
    /// 拖放文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DragFileDropHandler(object sender, DragEventArgs e) {
        e.Handled = true;
        if (e.Data.GetData(DataFormats.FileDrop) is IEnumerable<string> array) {
            if (!array.Any()) {
                return;
            }
            // 判断是否为文件
            if (File.Exists(array.First())) {
                FileName = array.First();
            }
        }
    }

    /// <summary>
    /// 文件拖放进入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DragFileEnterHandler(object sender, DragEventArgs e) {
        e.Handled = true;
        InputPanel.Background = (SolidColorBrush)Global.ColorResource["Gray3"];
        InputTextBox.Background = (SolidColorBrush)Global.ColorResource["Gray3"];
    }

    /// <summary>
    /// 更改 TextBox 默认拖拽行为
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FileDragOverHandler(object sender, DragEventArgs e) {
        e.Handled = true;
        e.Effects = DragDropEffects.Copy;
    }

    /// <summary>
    /// 设置鼠标移除时清除背景
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InputMouseLeaveHandler(object sender, System.Windows.Input.MouseEventArgs e) {
        e.Handled = true;
        InputPanel.Background = new SolidColorBrush(Colors.Transparent);
        InputTextBox.Background = new SolidColorBrush(Colors.Transparent);
    }

}
