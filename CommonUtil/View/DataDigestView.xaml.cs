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

    private class DigestInfo : DependencyObject {
        public static readonly DependencyProperty IsVivibleProperty = DependencyProperty.Register("IsVivible", typeof(bool), typeof(DigestInfo), new PropertyMetadata(false));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DigestInfo), new PropertyMetadata(""));
        public static readonly DependencyProperty ProcessProperty = DependencyProperty.Register("Process", typeof(double), typeof(DigestInfo), new PropertyMetadata(0.0));

        public DigestInfo(DataDigest.TextDigest textDigestHandler, DataDigest.StreamDigest streamDigestHandler) {
            TextDigest = textDigestHandler;
            StreamDigest = streamDigestHandler;
        }

        /// <summary>
        /// 文本 Hash 处理器
        /// </summary>
        public DataDigest.TextDigest TextDigest { get; set; }
        /// <summary>
        /// 文件流 Hash 处理器
        /// </summary>
        public DataDigest.StreamDigest StreamDigest { get; set; }
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
    private static readonly DependencyPropertyKey DigestInfoDictPropertyKey = DependencyProperty.RegisterReadOnly("DigestInfoDict", typeof(IDictionary<string, DigestInfo>), typeof(DataDigestView), new PropertyMetadata());
    private static readonly DependencyProperty DigestInfoDictProperty = DigestInfoDictPropertyKey.DependencyProperty;
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
    private IDictionary<string, DigestInfo> DigestInfoDict => (IDictionary<string, DigestInfo>)GetValue(DigestInfoDictProperty);
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
        SetValue(DigestInfoDictPropertyKey, DataSet.DigestOptions.ToDictionary(
            item => item.Item1,
            item => new DigestInfo(item.Item2, item.Item3)
        ));
        DigestAlgorithms = DigestInfoDict.Keys.ToArray();
        InitializeComponent();
        // 初始化 AlgorithmMenuFlyout
        this.SetLoadedOnceEventHandler((_, _) => {
            DigestAlgorithms.ForEach(item => {
                var menuItem = new MenuItem() {
                    Header = item,
                    Tag = item,
                    // 默认选项
                    IsChecked = item is "MD5" or "SHA256",
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
        } catch (TaskCanceledException) {
        } catch (FileNotFoundException error) {
            Logger.Error(error);
            MessageBoxUtils.Error("文件找不到！");
        } catch (IOException error) {
            Logger.Error(error);
            MessageBoxUtils.Error("文件读取失败！");
        } catch (Exception error) {
            Logger.Error(error);
            MessageBoxUtils.Error("处理失败！");
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
        var result = await Task.Run(() => info.TextDigest.Invoke(text));
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
            return info.StreamDigest(
                stream,
                CancellationTokenSource.Token,
                process => ThrottleUtils.Throttle($"{nameof(DataDigestView)}|{nameof(CalculateFileDigestAsync)}|{info.GetHashCode()}", () => {
                    Dispatcher.Invoke(() => info.Process = process);
                })
            );
        }, CancellationTokenSource.Token);
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
        MessageBoxUtils.Success("已复制");
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
