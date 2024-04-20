namespace CommonUtil.View;

public partial class RSACryptoControl : ResponsiveUserControl {
    public static readonly DependencyProperty SelectedAlgorithmProperty = DependencyProperty.Register("SelectedAlgorithm", typeof(string), typeof(RSACryptoControl), new PropertyMetadata());
    public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register("IsWorking", typeof(bool), typeof(RSACryptoControl), new PropertyMetadata(false));
    public static readonly DependencyProperty IsPublicKeyProperty = DependencyProperty.Register("IsPublicKey", typeof(bool), typeof(RSACryptoControl), new PropertyMetadata(true));

    public event RoutedEventHandler? EncryptClick;
    public event RoutedEventHandler? DecryptClick;
    public event RoutedEventHandler? CopyResultClick;
    public event RoutedEventHandler? ClearInputClick;

    public string SelectedAlgorithm {
        get { return (string)GetValue(SelectedAlgorithmProperty); }
        set { SetValue(SelectedAlgorithmProperty, value); }
    }
    public bool IsWorking {
        get { return (bool)GetValue(IsWorkingProperty); }
        set { SetValue(IsWorkingProperty, value); }
    }
    /// <summary>
    /// Whether the input is a public key or a private key.
    /// </summary>
    public bool IsPublicKey {
        get { return (bool)GetValue(IsPublicKeyProperty); }
        set { SetValue(IsPublicKeyProperty, value); }
    }

    public RSACryptoControl() : base(ResponsiveMode.Variable) {
        InitializeComponent();
        this.SetLoadedOnceEventHandler((_, _) => {
            AlgorithmsComboBox.SelectedIndex = 0;
        });
    }

    private void EncryptClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        EncryptClick?.Invoke(sender, e);
    }

    private void DecryptClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        DecryptClick?.Invoke(sender, e);
    }

    private void CopyResultClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        CopyResultClick?.Invoke(sender, e);
    }

    private void ClearInputClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        ClearInputClick?.Invoke(sender, e);
    }
}
