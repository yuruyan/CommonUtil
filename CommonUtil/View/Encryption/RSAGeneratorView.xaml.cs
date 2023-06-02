namespace CommonUtil.View;

public partial class RSAGeneratorView : ResponsivePage {
    public static readonly DependencyProperty PublicKeyProperty = DependencyProperty.Register("PublicKey", typeof(string), typeof(RSAGeneratorView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty PrivateKeyProperty = DependencyProperty.Register("PrivateKey", typeof(string), typeof(RSAGeneratorView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register("IsWorking", typeof(bool), typeof(RSAGeneratorView), new PropertyMetadata(false));
    private readonly IReadOnlyList<uint> StrengthOptions = new List<uint> {
        1024,
        1024 * 2,
        1024 * 3,
        1024 * 4,
    };

    /// <summary>
    /// 公钥
    /// </summary>
    public string PublicKey {
        get { return (string)GetValue(PublicKeyProperty); }
        set { SetValue(PublicKeyProperty, value); }
    }
    /// <summary>
    /// 私钥
    /// </summary>
    public string PrivateKey {
        get { return (string)GetValue(PrivateKeyProperty); }
        set { SetValue(PrivateKeyProperty, value); }
    }
    /// <summary>
    /// 是否在工作
    /// </summary>
    public bool IsWorking {
        get { return (bool)GetValue(IsWorkingProperty); }
        set { SetValue(IsWorkingProperty, value); }
    }

    public RSAGeneratorView() : base(ResponsiveMode.Variable) {
        InitializeComponent();
        StrengthComboBox.ItemsSource = StrengthOptions;
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void GenerateClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (IsWorking) {
            return;
        }
        IsWorking = true;
        ClearInput();
        if (StrengthComboBox.SelectedItem is not uint strength) {
            return;
        }
        var key = await Task.Run(() => TaskUtils.Try(() => {
            return RSACrypto.GenerateKey(strength);
        }));
        // Success
        if (key != default) {
            PublicKey = key.PublicKey;
            PrivateKey = key.PrivateKey;
        } else {
            MessageBoxUtils.Error("生成失败");
        }
        IsWorking = false;
    }

    /// <summary>
    /// 验证
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ValidateClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (IsWorking) {
            return;
        }
        // Check input
        if (string.IsNullOrEmpty(PublicKey) || string.IsNullOrEmpty(PrivateKey)) {
            MessageBoxUtils.Info("请输入密钥");
            return;
        }
        IsWorking = true;
        // Validate
        var result = await Task.Factory.StartNew(state => TaskUtils.Try<bool?>(() => {
            if (state is not ValueTuple<string, string> tuple) {
                return null;
            }
            var (pub, pri) = tuple;
            var randomData = Encoding.UTF8.GetBytes($"{Random.Shared.NextDouble()}");
            var encrypted = RSACrypto.Encrypt(pub, randomData, RSAAlgorithm.RSA);
            var decrypted = RSACrypto.Decrypt(pri, encrypted, RSAAlgorithm.RSA);
            return Enumerable.SequenceEqual(randomData, decrypted);
        }), (PublicKey, PrivateKey));
        // Success
        if (result is true) {
            MessageBoxUtils.Success("验证成功");
        } else {
            MessageBoxUtils.Error("验证失败");
        }
        IsWorking = false;
    }

    /// <summary>
    /// Copy output
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        //"-----BEGIN PUBLIC KEY-----": PKCS#8 格式公钥
        //"-----BEGIN PRIVATE KEY-----": PKCS#8 格式私钥
        Clipboard.SetDataObject(
            $"-----BEGIN PUBLIC KEY-----\n"
            + $"{PublicKey}\n"
            + $"-----END PUBLIC KEY-----\n"
            + $"-----BEGIN PRIVATE KEY-----\n"
            + $"{PrivateKey}\n"
            + $"-----END PRIVATE KEY-----\n"
        );
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// Clear input
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        ClearInput();
    }

    /// <summary>
    /// 清除输入
    /// </summary>
    private void ClearInput() {
        PublicKey = PrivateKey = string.Empty;
    }
}
