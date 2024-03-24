using System.Text.RegularExpressions;

namespace CommonUtil.View;

public partial class AESCryptoView : ResponsivePage {
    public enum TextFormat {
        UTF8,
        Base64,
        Hex,
    }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IReadOnlyList<AESCryptoMode> CryptoModes = Enum.GetValues<AESCryptoMode>();
    private readonly IReadOnlyList<AESPaddingMode> PaddingModes = Enum.GetValues<AESPaddingMode>();
    private readonly IReadOnlyList<TextFormat> TextFormats = Enum.GetValues<TextFormat>();
    public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(AESCryptoView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IvProperty = DependencyProperty.Register("Iv", typeof(string), typeof(AESCryptoView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(AESCryptoView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(AESCryptoView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty CryptoModeProperty = DependencyProperty.Register("CryptoMode", typeof(AESCryptoMode), typeof(AESCryptoView), new PropertyMetadata(AESCryptoMode.ECB));
    public static readonly DependencyProperty PaddingModeProperty = DependencyProperty.Register("PaddingMode", typeof(AESPaddingMode), typeof(AESCryptoView), new PropertyMetadata(AESPaddingMode.PKCS7Padding));
    public static readonly DependencyProperty OutputFormatProperty = DependencyProperty.Register("OutputFormat", typeof(TextFormat), typeof(AESCryptoView), new PropertyMetadata(TextFormat.Base64));
    public static readonly DependencyProperty InputFormatProperty = DependencyProperty.Register("InputFormat", typeof(TextFormat), typeof(AESCryptoView), new PropertyMetadata(TextFormat.UTF8));
    private static readonly DependencyProperty IsEncryptingProperty = DependencyProperty.Register("IsEncrypting", typeof(bool), typeof(AESCryptoView), new PropertyMetadata(false));
    private static readonly DependencyProperty IsDecryptingProperty = DependencyProperty.Register("IsDecrypting", typeof(bool), typeof(AESCryptoView), new PropertyMetadata(false));
    public static readonly DependencyPropertyKey FileProcessStatusesPropertyKey = DependencyProperty.RegisterReadOnly("FileProcessStatuses", typeof(ObservableCollection<FileProcessStatus>), typeof(AESCryptoView), new PropertyMetadata());
    public static readonly DependencyProperty FileProcessStatusesProperty = FileProcessStatusesPropertyKey.DependencyProperty;
    public static readonly DependencyProperty IsExpanded2Property = DependencyProperty.Register("IsExpanded2", typeof(bool), typeof(AESCryptoView), new PropertyMetadata(false));
    private readonly string DescriptionHeaderAutoWidthGroupId;
    /// <summary>
    /// 保存文件对话框
    /// </summary>
    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "All Files|*.*"
    };
    /// <summary>
    /// 保存目录对话框
    /// </summary>
    private readonly VistaFolderBrowserDialog SaveDirectoryDialog = new() {
        Description = "选择保存目录",
        UseDescriptionForTitle = true
    };
    /// <summary>
    /// 当前 Window
    /// </summary>
    private Window CurrentWindow = App.Current.MainWindow;
    private CancellationTokenSource EncryptionCancellationTokenSource = new();
    private CancellationTokenSource DecryptionCancellationTokenSource = new();
    private readonly double ExpansionThreshold2;

    private readonly Regex KeyRegex = GetKeyRegex();
    private readonly Regex IvRegex = GetIvRegex();
    [GeneratedRegex("^((?:[a-z0-9]{32})|(?:[a-z0-9]{48})|(?:[a-z0-9]{64}))$", RegexOptions.IgnoreCase)]
    private static partial Regex GetKeyRegex();
    [GeneratedRegex("^[a-z0-9]{32}$", RegexOptions.IgnoreCase)]
    private static partial Regex GetIvRegex();

    /// <summary>
    /// 文件处理列表
    /// </summary>
    public ObservableCollection<FileProcessStatus> FileProcessStatuses => (ObservableCollection<FileProcessStatus>)GetValue(FileProcessStatusesProperty);
    public string Key {
        get { return (string)GetValue(KeyProperty); }
        set { SetValue(KeyProperty, value); }
    }
    public string Iv {
        get { return (string)GetValue(IvProperty); }
        set { SetValue(IvProperty, value); }
    }
    /// <summary>
    /// 输入
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }
    /// <summary>
    /// 输出
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }
    public AESCryptoMode CryptoMode {
        get { return (AESCryptoMode)GetValue(CryptoModeProperty); }
        set { SetValue(CryptoModeProperty, value); }
    }
    public AESPaddingMode PaddingMode {
        get { return (AESPaddingMode)GetValue(PaddingModeProperty); }
        set { SetValue(PaddingModeProperty, value); }
    }
    public TextFormat OutputFormat {
        get { return (TextFormat)GetValue(OutputFormatProperty); }
        set { SetValue(OutputFormatProperty, value); }
    }
    public TextFormat InputFormat {
        get { return (TextFormat)GetValue(InputFormatProperty); }
        set { SetValue(InputFormatProperty, value); }
    }
    /// <summary>
    /// 是否正在加密
    /// </summary>
    private bool IsEncrypting {
        get { return (bool)GetValue(IsEncryptingProperty); }
        set { SetValue(IsEncryptingProperty, value); }
    }
    /// <summary>
    /// 是否正在解密
    /// </summary>
    private bool IsDecrypting {
        get { return (bool)GetValue(IsDecryptingProperty); }
        set { SetValue(IsDecryptingProperty, value); }
    }
    public bool IsExpanded2 {
        get { return (bool)GetValue(IsExpanded2Property); }
        set { SetValue(IsExpanded2Property, value); }
    }

    public AESCryptoView() {
        SetValue(FileProcessStatusesPropertyKey, new ObservableCollection<FileProcessStatus>());
        DescriptionHeaderAutoWidthGroupId = $"{nameof(AESCryptoView)}_{GetHashCode()}_DescriptionHeader";
        InitializeComponent();
        ExpansionThreshold2 = (double)Resources["ExpansionThreshold2"];
        CryptoModeComboBox.ItemsSource = CryptoModes;
        PaddingModeComboBox.ItemsSource = PaddingModes;
        InputFormatComboBox.ItemsSource = OutputFormatComboBox.ItemsSource = TextFormats;
        this.SetLoadedOnceEventHandler(static (sender, _) => {
            if (sender is AESCryptoView self) {
                self.CurrentWindow = Window.GetWindow(self);
            }
        });
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void EncryptClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 正在加密
        if (IsEncrypting) {
            return;
        }
        // 输入检查
        if (!CheckInputValidation()) {
            return;
        }
        var hasFile = DragDropTextBox.HasFile;

        // 处理文本
        if (!hasFile) {
            EncryptText();
            return;
        }

        EncryptionCancellationTokenSource.Dispose();
        EncryptionCancellationTokenSource = new();
        IsEncrypting = true;
        await EncryptFile();
        IsEncrypting = false;
    }

    private async void DecryptClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 正在解密
        if (IsDecrypting) {
            return;
        }
        // 输入检查
        if (!CheckInputValidation()) {
            return;
        }
        var hasFile = DragDropTextBox.HasFile;

        // 处理文本
        if (!hasFile) {
            DecryptText();
            return;
        }

        DecryptionCancellationTokenSource.Dispose();
        DecryptionCancellationTokenSource = new();
        IsDecrypting = true;
        await DecryptFile();
        IsDecrypting = false;
    }

    /// <summary>
    /// 加密文件
    /// </summary>
    [NoException]
    private async Task EncryptFile() {
        var fileNames = DragDropTextBox.FileNames;
        if (fileNames.Count == 1) {
            await EncryptOneFile(fileNames[0]);
        } else {
            await EncryptMultiFiles(fileNames);
        }
    }

    /// <summary>
    /// 解密文件
    /// </summary>
    [NoException]
    private async Task DecryptFile() {
        var fileNames = DragDropTextBox.FileNames;
        if (fileNames.Count == 1) {
            await DecryptOneFile(fileNames[0]);
        } else {
            await DecryptMultiFiles(fileNames);
        }
    }

    /// <summary>
    /// 加密单个文件
    /// </summary>
    /// <returns></returns>
    [NoException]
    private async Task EncryptOneFile(string filename) {
        await ProcessOneFile(filename, true);
    }

    /// <summary>
    /// 解密单个文件
    /// </summary>
    /// <returns></returns>
    [NoException]
    private async Task DecryptOneFile(string filename) {
        await ProcessOneFile(filename, false);
    }

    /// <summary>
    /// 处理单文件
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="isEncryption"></param>
    /// <returns></returns>
    private async Task ProcessOneFile(string filename, bool isEncryption) {
        var (source, func) = ((CancellationTokenSource, CommonFileProcess))(isEncryption ?
            (EncryptionCancellationTokenSource, EncryptFile)
            : (DecryptionCancellationTokenSource, DecryptFile)
        );
        await FileProcessUtils.ProcessOneFileAsync(
            filename,
            SaveFileDialog,
            source,
            FileProcessStatuses,
            func,
            Logger
        );
    }

    private void DecryptFile(string inputFile, string outputFile, CancellationToken? token = null, Action<double>? callback = null) {
        ProcessFile(inputFile, outputFile, false, token, callback);
    }

    private void EncryptFile(string inputFile, string outputFile, CancellationToken? token = null, Action<double>? callback = null) {
        ProcessFile(inputFile, outputFile, true, token, callback);
    }

    /// <summary>
    /// 处理单个文件
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="outputFile"></param>
    /// <param name="isEncryption"></param>
    /// <param name="token"></param>
    /// <param name="callback"></param>
    private void ProcessFile(
        string inputFile,
        string outputFile,
        bool isEncryption,
        CancellationToken? token = null,
        Action<double>? callback = null
    ) {
        var (cryptoMode, paddingMode, key, iv, isIvChecked) = Dispatcher.Invoke(() => {
            return (CryptoMode, PaddingMode, Key, Iv, IvCheckBox.IsChecked is true);
        });
        var func = (Action<AESCryptoMode, AESPaddingMode, string, string, byte[], byte[]?, CancellationToken?, Action<double>?>)
                    (isEncryption ? AESCryptoUtils.EncryptFile : AESCryptoUtils.DecryptFile);
        func(
            cryptoMode,
            paddingMode,
            inputFile,
            outputFile,
            ParseKey(key),
            ParseIv(iv, cryptoMode, isIvChecked),
            token,
            callback
        );
    }

    /// <summary>
    /// 加密多个文件
    /// </summary>
    /// <param name="filenames"></param>
    /// <returns></returns>
    [NoException]
    private async Task EncryptMultiFiles(ICollection<string> filenames) {
        await ProcessMultiFiles(filenames, true);
    }

    /// <summary>
    /// 解密多个文件
    /// </summary>
    /// <param name="filenames"></param>
    /// <returns></returns>
    [NoException]
    private async Task DecryptMultiFiles(ICollection<string> filenames) {
        await ProcessMultiFiles(filenames, false);
    }

    /// <summary>
    /// 处理多文件
    /// </summary>
    /// <param name="filenames"></param>
    /// <param name="isEncryption"></param>
    /// <returns></returns>
    [NoException]
    private async Task ProcessMultiFiles(ICollection<string> filenames, bool isEncryption) {
        var (source, func) = ((CancellationTokenSource, CommonFileProcess))(isEncryption ?
             (EncryptionCancellationTokenSource, EncryptFile)
             : (DecryptionCancellationTokenSource, DecryptFile));

        await FileProcessUtils.ProcessMultiFilesAsync(
            filenames,
            SaveDirectoryDialog,
            source,
            FileProcessStatuses,
            func,
            CurrentWindow,
            Logger
        );
    }

    /// <summary>
    /// 解析输入
    /// </summary>
    /// <param name="input"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <remarks>
    /// Included message notification
    /// </remarks>
    private bool TryParseInputText(string input, out byte[] data) {
        data = Array.Empty<byte>();
        try {
            data = InputFormat switch {
                TextFormat.UTF8 => Encoding.UTF8.GetBytes(input),
                TextFormat.Hex => Convert.FromHexString(input),
                TextFormat.Base64 => Convert.FromBase64String(input),
                _ => throw new FormatException()
            };
            return true;
        } catch {
            MessageBoxUtils.Error("解析输入失败");
        }
        return false;
    }

    /// <summary>
    /// 格式化输出
    /// </summary>
    /// <param name="data"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <remarks>
    /// Included message notification
    /// </remarks>
    private bool TryFormatOutput(byte[] data, out string result) {
        result = string.Empty;
        try {
            result = OutputFormat switch {
                TextFormat.UTF8 => Encoding.UTF8.GetString(data),
                TextFormat.Hex => Convert.ToHexString(data),
                TextFormat.Base64 => Convert.ToBase64String(data),
                _ => throw new FormatException()
            };
            return true;
        } catch {
            MessageBoxUtils.Error("格式化输出失败");
        }
        return false;
    }

    private static byte[] ParseKey(string key) => Convert.FromHexString(key);

    private static byte[]? ParseIv(string iv, AESCryptoMode mode, bool isIvChecked) {
        return mode == AESCryptoMode.ECB
            ? null
            : mode == AESCryptoMode.CTR || isIvChecked
                ? Convert.FromHexString(iv)
                : null;
    }

    /// <summary>
    /// 加密文本
    /// </summary>
    [NoException]
    private void EncryptText() {
        if (!TryParseInputText(InputText, out byte[] data)) {
            return;
        }
        try {
            var output = AESCryptoUtils.Encrypt(
                CryptoMode,
                PaddingMode,
                data,
                ParseKey(Key),
                ParseIv(Iv, CryptoMode, IvCheckBox.IsChecked is true)
            );
            if (TryFormatOutput(output, out var result)) {
                OutputText = result;
            }
        } catch {
            MessageBoxUtils.Error("加密失败");
        }
    }

    /// <summary>
    /// 解密文本
    /// </summary>
    [NoException]
    private void DecryptText() {
        if (!TryParseInputText(InputText, out byte[] data)) {
            return;
        }
        try {
            var output = AESCryptoUtils.Decrypt(
                CryptoMode,
                PaddingMode,
                data,
                ParseKey(Key),
                ParseIv(Iv, CryptoMode, IvCheckBox.IsChecked is true)
            );
            if (TryFormatOutput(output, out var result)) {
                OutputText = result;
            }
        } catch {
            MessageBoxUtils.Error("解密失败");
        }
    }

    /// <summary>
    /// 检查输入合法性
    /// </summary>
    /// <returns></returns>
    private bool CheckInputValidation() {
        if (string.IsNullOrEmpty(Key)) {
            MessageBoxUtils.Error("Key 不能为空");
            return false;
        }
        if (!KeyRegex.IsMatch(Key)) {
            MessageBoxUtils.Error("Key 无效");
            return false;
        }
        // Skip checking
        if (CryptoMode == AESCryptoMode.ECB) {
            return true;
        }
        if (CryptoMode == AESCryptoMode.CTR || IvCheckBox.IsChecked is true) {
            if (string.IsNullOrEmpty(Iv)) {
                MessageBoxUtils.Error("Iv 不能为空");
                return false;
            }
            if (!IvRegex.IsMatch(Iv)) {
                MessageBoxUtils.Error("Iv 无效");
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Copy output
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// Clear input
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 没有正在处理的任务
        if (!IsEncrypting && !IsDecrypting) {
            FileProcessStatuses.Clear();
        }
        DragDropTextBox.Clear();
        InputText = OutputText = string.Empty;
    }

    /// <summary>
    /// Update IvCheckBox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CryptoModeComboBoxSelectionChangedHandler(object sender, SelectionChangedEventArgs e) {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is AESCryptoMode mode) {
            if (mode == AESCryptoMode.CTR) {
                IvCheckBox.IsChecked = true;
            } else if (mode == AESCryptoMode.ECB) {
                IvCheckBox.IsChecked = false;
            }
        }
    }

    private void ViewLoadedHandler(object sender, RoutedEventArgs e) {
        AutoSizeHelper.SetAutoWidth(
            DescriptionHeaderAutoWidthGroupId,
            this
                .FindDescendantsBy(obj => {
                    return obj is FrameworkElement element && element.Tag?.ToString() == "DescriptionHeader";
                }).Cast<FrameworkElement>()
        );
    }

    private void ViewUnloadedHandler(object sender, RoutedEventArgs e) {
        AutoSizeHelper.DisableAutoWidth(DescriptionHeaderAutoWidthGroupId);
    }

    /// <summary>
    /// 取消加密
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelEncryptClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        EncryptionCancellationTokenSource.Cancel();
    }

    /// <summary>
    /// 取消解密
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelDecryptClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        DecryptionCancellationTokenSource.Cancel();
    }

    /// <summary>
    /// SizeChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ElementSizeChangedHandler(object sender, SizeChangedEventArgs e) {
        IsExpanded2 = e.NewSize.Width >= ExpansionThreshold2;
    }
}
