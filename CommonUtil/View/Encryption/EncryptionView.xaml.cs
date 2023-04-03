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
        // Cannot set on 'this' or 'NavigationView'
        AESCryptoView.SetLoadedOnceEventHandler((_, _) => {
            NavigationView.MenuItems
               .OfType<ModernWpf.Controls.NavigationViewItem>()
               .ForEach(item => item.SetIconSize(20));
        });
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
