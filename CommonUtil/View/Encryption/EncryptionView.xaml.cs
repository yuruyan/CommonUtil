namespace CommonUtil.View;

public partial class EncryptionView : Page {
    private readonly Type[] Routers = {
        typeof(AESCryptoView),
    };

    public EncryptionView() {
        InitializeComponent();
    }

    private void ViewLoadedHandler(object sender, RoutedEventArgs e) {
        NavigationUtils.EnableNavigation(
            NavigationView,
            new(ContentFrame, Routers),
            ContentFrame
        );
    }

    private void ViewUnloadedHandler(object sender, RoutedEventArgs e) {
        NavigationUtils.DisableNavigation(NavigationView);
    }
}
