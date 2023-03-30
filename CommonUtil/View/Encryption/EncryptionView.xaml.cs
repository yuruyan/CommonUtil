namespace CommonUtil.View;

public partial class EncryptionView : Page {
    private readonly RouterService RouterService;
    private readonly Type[] Routers = {
        typeof(AESCryptoView),
        typeof(RSAGeneratorView),
        typeof(RSACryptoView),
    };

    public EncryptionView() {
        InitializeComponent();
        RouterService = new(ContentFrame, Routers);
    }

    private void ViewLoadedHandler(object sender, RoutedEventArgs e) {
        NavigationUtils.EnableNavigation(
            NavigationView,
            RouterService,
            ContentFrame
        );
    }

    private void ViewUnloadedHandler(object sender, RoutedEventArgs e) {
        NavigationUtils.DisableNavigation(NavigationView);
    }
}
