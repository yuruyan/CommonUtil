﻿namespace CommonUtil.View;

public partial class RSACryptoView : Page {
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(RSACryptoView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(RSACryptoView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RSACryptoView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register("IsWorking", typeof(bool), typeof(RSACryptoView), new PropertyMetadata(false));
    public static readonly DependencyProperty SelectedAlgorithmProperty = DependencyProperty.Register("SelectedAlgorithm", typeof(string), typeof(RSACryptoView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IsPublicKeyProperty = DependencyProperty.Register("IsPublicKey", typeof(bool), typeof(RSACryptoView), new PropertyMetadata(true));

    /// <summary>
    /// 是否是公钥
    /// </summary>
    public bool IsPublicKey {
        get { return (bool)GetValue(IsPublicKeyProperty); }
        set { SetValue(IsPublicKeyProperty, value); }
    }
    /// <summary>
    /// 输入数据
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }
    /// <summary>
    /// 密钥
    /// </summary>
    public string Key {
        get { return (string)GetValue(KeyProperty); }
        set { SetValue(KeyProperty, value); }
    }
    /// <summary>
    /// 处理数据
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }
    /// <summary>
    /// 是否正在处理
    /// </summary>
    public bool IsWorking {
        get { return (bool)GetValue(IsWorkingProperty); }
        set { SetValue(IsWorkingProperty, value); }
    }
    /// <summary>
    /// Algorithm
    /// </summary>
    public string SelectedAlgorithm {
        get { return (string)GetValue(SelectedAlgorithmProperty); }
        set { SetValue(SelectedAlgorithmProperty, value); }
    }

    public RSACryptoView() {
        InitializeComponent();
    }

    /// <summary>
    /// Copy output
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// Clear input
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        InputText = OutputText = Key = string.Empty;
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void EncryptClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (IsWorking || !CheckInputValidation()) {
            return;
        }
        OutputText = string.Empty;
        IsWorking = true;
        var encrypted = await DoCrypto(true, IsPublicKey);
        // Success
        if (encrypted is not null) {
            OutputText = encrypted;
        } else {
            MessageBoxUtils.Error("加密失败");
        }
        IsWorking = false;
    }

    /// <summary>
    /// 处理
    /// </summary>
    /// <param name="isEncryption"></param>
    /// <param name="isPublicKey"></param>
    /// <returns></returns>
    private async Task<string?> DoCrypto(bool isEncryption, bool isPublicKey) {
        return await Task.Factory.StartNew(state => TaskUtils.Try(() => {
            if (state is not ValueTuple<string, string, string> tuple) {
                return null;
            }

            var (key, data, algorithm) = tuple;
            var func = (Func<string, byte[], string, bool, byte[]>)(
                isEncryption ? RSACrypto.Encrypt : RSACrypto.Decrypt
            );
            return Convert.ToBase64String(func(key, Convert.FromBase64String(data), algorithm, isPublicKey));
        }), (Key, InputText, SelectedAlgorithm));
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DecryptClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (IsWorking || !CheckInputValidation()) {
            return;
        }
        OutputText = string.Empty;
        IsWorking = true;
        var decrypted = await DoCrypto(false, IsPublicKey);
        // Success
        if (decrypted is not null) {
            OutputText = decrypted;
        } else {
            MessageBoxUtils.Error("解密失败");
        }
        IsWorking = false;
    }

    /// <summary>
    /// 检查输入合法性
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// With notifying
    /// </remarks>
    private bool CheckInputValidation() {
        if (string.IsNullOrEmpty(InputText)) {
            MessageBoxUtils.Info("请输入数据");
            return false;
        }
        if (string.IsNullOrEmpty(Key)) {
            MessageBoxUtils.Info("请输入密钥");
            return false;
        }
        return true;
    }
}
