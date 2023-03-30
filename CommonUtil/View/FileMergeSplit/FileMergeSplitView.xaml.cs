namespace CommonUtil.View;

public partial class FileMergeSplitView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly RouterService RouterService;
    private readonly Type[] Routers = {
        typeof(FileMergeView),
        typeof(FileSplitView),
    };

    public FileMergeSplitView() {
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
