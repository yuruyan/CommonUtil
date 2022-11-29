using CommonUITools.Utils;
using CommonUtil.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CommonUtil.View;

/// <summary>
/// 等于 0 或等于 1 时隐藏
/// </summary>
internal class ProcessVisibilityConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var v = System.Convert.ToDouble(value);
        if (v == 0 || v == 1) {
            return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

public partial class DataDigestView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private delegate string? TextDigestHandler(string digest);
    /// <summary>
    /// StreamDigestHandler
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    private delegate string? StreamDigestHandler(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null);

    private class DigestInfo : DependencyObject {
        public static readonly DependencyProperty IsVivibleProperty = DependencyProperty.Register("IsVivible", typeof(bool), typeof(DigestInfo), new PropertyMetadata(false));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DigestInfo), new PropertyMetadata(""));
        public static readonly DependencyProperty ProcessProperty = DependencyProperty.Register("Process", typeof(double), typeof(DigestInfo), new PropertyMetadata(0.0));

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
        public double Process {
            get { return (double)GetValue(ProcessProperty); }
            set { SetValue(ProcessProperty, value); }
        }
        /// <summary>
        /// 文件总大小
        /// </summary>
        public long FileTotalSize { get; set; } = 0;
    }

    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(DataDigestView), new PropertyMetadata(""));
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(DataDigestView), new PropertyMetadata(false));
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
    /// 是否有文件
    /// </summary>
    public bool HasFile {
        get { return (bool)GetValue(HasFileProperty); }
        set { SetValue(HasFileProperty, value); }
    }
    /// <summary>
    /// 摘要算法 Dict
    /// </summary>
    private Dictionary<string, DigestInfo> DigestInfoDict {
        get { return (Dictionary<string, DigestInfo>)GetValue(DigestInfoDictProperty); }
        set { SetValue(DigestInfoDictProperty, value); }
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
    /// 散列算法选择
    /// </summary>
    private readonly IList<string> DigestAlgorithms;
    private CancellationTokenSource CancellationTokenSource = new();

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
            { "WhirlpoolDigest", new(DataDigest.WhirlpoolDigest, DataDigest.WhirlpoolDigest) },
            { "TigerDigest", new(DataDigest.TigerDigest, DataDigest.TigerDigest) },
            { "SM3Digest", new(DataDigest.SM3Digest, DataDigest.SM3Digest) },
            { "ShakeDigest", new(DataDigest.ShakeDigest, DataDigest.ShakeDigest) },
            { "RipeMD128Digest", new(DataDigest.RipeMD128Digest, DataDigest.RipeMD128Digest) },
            { "RipeMD160Digest", new(DataDigest.RipeMD160Digest, DataDigest.RipeMD160Digest) },
            { "RipeMD256Digest", new(DataDigest.RipeMD256Digest, DataDigest.RipeMD256Digest) },
            { "RipeMD320Digest", new(DataDigest.RipeMD320Digest, DataDigest.RipeMD320Digest) },
            { "KeccakDigest", new(DataDigest.KeccakDigest, DataDigest.KeccakDigest) },
            { "Gost3411Digest", new(DataDigest.Gost3411Digest, DataDigest.Gost3411Digest) },
            { "Gost3411_2012_256Digest", new(DataDigest.Gost3411_2012_256Digest, DataDigest.Gost3411_2012_256Digest) },
            { "Gost3411_2012_512Digest", new(DataDigest.Gost3411_2012_512Digest, DataDigest.Gost3411_2012_512Digest) },
            { "Blake2bDigest", new(DataDigest.Blake2bDigest, DataDigest.Blake2bDigest) },
            { "Blake2sDigest", new(DataDigest.Blake2sDigest, DataDigest.Blake2sDigest) },
        };
        DigestAlgorithms = DigestInfoDict.Keys.ToArray();
        InitializeComponent();
        // 初始化 AlgorithmMenuFlyout
        UIUtils.SetLoadedOnceEventHandler(this, (_, _) => {
            DigestAlgorithms.ForEach(item => {
                var menuItem = new MenuItem() {
                    Header = item,
                    Tag = item,
                    // 默认选项
                    IsChecked = item == "MD5" || item == "SHA256",
                };
                // 防止点击关闭
                menuItem.PreviewMouseUp += (sender, e) => {
                    e.Handled = true;
                    if (sender is MenuItem menu) {
                        menu.IsChecked = !menu.IsChecked;
                    }
                };
                AlgorithmMenuFlyout.Items.Add(menuItem);
            });
        });
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
    private async void StartClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (IsWorking) {
            return;
        }
        IsWorking = true;
        CancellationTokenSource.Dispose();
        CancellationTokenSource = new();
        try {
            await CalculateDigest();
        } catch (FileNotFoundException error) {
            Logger.Error(error);
            CommonUITools.Widget.MessageBox.Error("文件找不到！");
        } catch (IOException error) {
            Logger.Error(error);
            CommonUITools.Widget.MessageBox.Error("文件读取失败！");
        } catch (Exception error) {
            Logger.Error(error);
            CommonUITools.Widget.MessageBox.Error("处理失败！");
        }
        IsWorking = false;
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
        foreach (var item in digests) {
            item.Text = string.Empty;
            item.Process = 0;
        }
        var tasks = new List<Task>();
        // 文件处理
        tasks.AddRange(
            HasFile ? digests.Select(info => CalculateFileDigestAsync(info, FileName))
            : digests.Select(info => CalculateTextDigestAsync(info, InputText))
        );
        // 等待全部完成
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 计算文本摘要
    /// </summary>
    /// <param name="info"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    private async Task CalculateTextDigestAsync(DigestInfo info, string text) {
        var result = await Task.Run(() => info.TextDigestHandler.Invoke(text));
        info.Process = 1;
        info.Text = result ?? string.Empty;
    }

    /// <summary>
    /// 计算文件摘要
    /// </summary>
    /// <param name="info"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    private async Task CalculateFileDigestAsync(DigestInfo info, string filename) {
        var result = await Task.Run(() => {
            using var stream = File.OpenRead(filename);
            return info.StreamDigestHandler(
                stream,
                CancellationTokenSource.Token,
                process => ThrottleUtils.Throttle(info, () => {
                    Dispatcher.Invoke(() => info.Process = process);
                })
            );
        });
        info.Process = 1;
        info.Text = result ?? string.Empty;
    }

    /// <summary>
    /// 取消计算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StopDigestClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        CancellationTokenSource.Cancel();
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
