﻿using CommonUITools.Utils;
using CommonUtil.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
        /// 文件总大小
        /// </summary>
        public long FileTotalSize { get; set; } = 0;
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
    private static readonly DependencyProperty DigestInfoDictProperty = DependencyProperty.Register("DigestInfoDict", typeof(Dictionary<string, DigestInfo>), typeof(DataDigestView), new PropertyMetadata());
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(DataDigestView), new PropertyMetadata(""));
    public static readonly DependencyProperty RunningProcessProperty = DependencyProperty.Register("RunningProcess", typeof(int), typeof(DataDigestView), new PropertyMetadata(0));
    public static readonly DependencyProperty FileSizeProperty = DependencyProperty.Register("FileSize", typeof(long), typeof(DataDigestView), new PropertyMetadata(0L));
    public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register("IsWorking", typeof(bool), typeof(DataDigestView), new PropertyMetadata(false));

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
    /// 文件大小
    /// </summary>
    public long FileSize {
        get { return (long)GetValue(FileSizeProperty); }
        set { SetValue(FileSizeProperty, value); }
    }
    /// <summary>
    /// 停止按钮是否可见
    /// </summary>
    public bool IsWorking {
        get { return (bool)GetValue(IsWorkingProperty); }
        set { SetValue(IsWorkingProperty, value); }
    }
    /// <summary>
    /// 正在工作的 stream
    /// </summary>
    private readonly ICollection<FileStream> WorkingDigestStream = new List<FileStream>();
    /// <summary>
    /// 散列算法选择
    /// </summary>
    private readonly IList<string> DigestAlgorithms;

    public DataDigestView() {
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
        DigestAlgorithms = DigestInfoDict.Keys.ToArray();
        InitializeComponent();
        // 初始化 AlgorithmMenuFlyout
        UIUtils.SetLoadedOnceEventHandler(this, (_, _) => {
            DigestAlgorithms.ForEach(item => AlgorithmMenuFlyout.Items.Add(
                new MenuItem() {
                    Header = item,
                    Tag = item,
                    // 默认选项
                    IsChecked = item == "MD5" || item == "SHA256",
                }
            ));
        });
        DependencyPropertyDescriptor
            .FromProperty(FileNameProperty, typeof(DataDigestView))
            .AddValueChanged(this, FileNameChangedHandler);
    }

    private void FileNameChangedHandler(object? sender, EventArgs e) {
        if (string.IsNullOrEmpty(FileName)) {
            return;
        }
        FileSize = new FileInfo(FileName).Length;
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
        DragDropTextBox.Clear();
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
            IsWorking = true;
            await CalculateDigest();
        });
    }

    /// <summary>
    /// 计算 Hash
    /// </summary>
    /// <returns></returns>
    private async Task CalculateDigest() {
        var digestInfoSet = AlgorithmMenuFlyout.Items
            .Cast<MenuItem>()
            .Where(item => item.IsChecked)
            .Select(item => DigestInfoDict[(string)item.Tag])
            .ToHashSet();
        // 设置 Visibility
        DigestInfoDict.ForEach(kv => kv.Value.IsVivible = digestInfoSet.Contains(kv.Value));
        // 计算
        await CalculateDigest(digestInfoSet);
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
                item.FileTotalSize = FileSize;
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
        IsWorking = false;
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
                        info.Process = (int)(100 * (double)read / info.FileTotalSize);
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
        IsWorking = false;
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
            }
        }
    }
}
