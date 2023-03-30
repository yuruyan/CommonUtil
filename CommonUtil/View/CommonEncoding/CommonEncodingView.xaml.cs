namespace CommonUtil.View;

public partial class CommonEncodingView : Page {
    //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly RouterService RouterService;
    private readonly Type[] Routers = {
        typeof(Base64EncodingView),
        typeof(UnicodeEncodingView),
        typeof(UTF8EncodingView),
        typeof(URLEncodingView),
        typeof(HexEncodingView),
    };

    public CommonEncodingView() {
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
