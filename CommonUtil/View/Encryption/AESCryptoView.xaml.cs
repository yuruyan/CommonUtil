using System.Text.RegularExpressions;

namespace CommonUtil.View;

public partial class AESCryptoView : ResponsivePage {
    public enum TextFormat {
        UTF8,
        Base64,
        Hex,
    }

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
    private readonly string DescriptionHeaderAutoWidthGroupId;
#if NET7_0_OR_GREATER
    private readonly Regex KeyRegex = GetKeyRegex();
    private readonly Regex IvRegex = GetIvRegex();
    [GeneratedRegex("^((?:[a-z0-9]{32})|(?:[a-z0-9]{48})|(?:[a-z0-9]{64}))$", RegexOptions.IgnoreCase)]
    private static partial Regex GetKeyRegex();
    [GeneratedRegex("^[a-z0-9]{32}$", RegexOptions.IgnoreCase)]
    private static partial Regex GetIvRegex();
#elif NET6_0_OR_GREATER
    private readonly Regex KeyRegex = new(@"^((?:[a-z0-9]{32})|(?:[a-z0-9]{48})|(?:[a-z0-9]{64}))$", RegexOptions.IgnoreCase);
    private readonly Regex IvRegex = new(@"^[a-z0-9]{32}$", RegexOptions.IgnoreCase);
#endif

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

    public AESCryptoView() {
        DescriptionHeaderAutoWidthGroupId = $"{nameof(AESCryptoView)}_{GetHashCode()}_DescriptionHeader";
        InitializeComponent();
        CryptoModeComboBox.ItemsSource = CryptoModes;
        PaddingModeComboBox.ItemsSource = PaddingModes;
        InputFormatComboBox.ItemsSource = OutputFormatComboBox.ItemsSource = TextFormats;
    }

    private void EncryptClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (!CheckInputValidation()) {
            return;
        }
        EncryptText();
    }

    private void DecryptClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (!CheckInputValidation()) {
            return;
        }
        DecryptText();
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
                Convert.FromHexString(Key),
                CryptoMode != AESCryptoMode.ECB && (CryptoMode == AESCryptoMode.CTR || IvCheckBox.IsChecked is true)
                    ? Convert.FromHexString(Iv) : null
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
                Convert.FromHexString(Key),
                CryptoMode != AESCryptoMode.ECB && (CryptoMode == AESCryptoMode.CTR || IvCheckBox.IsChecked is true)
                    ? Convert.FromHexString(Iv) : null
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
        InputText = OutputText = Key = Iv = string.Empty;
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
}
