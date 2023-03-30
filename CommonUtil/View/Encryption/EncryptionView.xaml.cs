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
        NavigationUtils.EnableNavigationPanelResponsive(NavigationView);
    }

    private void ViewUnloadedHandler(object sender, RoutedEventArgs e) {
        NavigationUtils.DisableNavigation(NavigationView);
        NavigationUtils.DisableNavigationPanelResponsive(NavigationView);
    }
}
