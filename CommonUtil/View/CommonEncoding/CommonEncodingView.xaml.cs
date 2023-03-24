namespace CommonUtil.View;

public partial class CommonEncodingView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly Type[] Routers = {
        typeof(UnicodeEncodingView),
        typeof(UTF8EncodingView),
        typeof(URLEncodingView),
        typeof(HexEncodingView),
    };

    public CommonEncodingView() {
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
