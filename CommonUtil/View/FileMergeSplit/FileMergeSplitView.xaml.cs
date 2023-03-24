namespace CommonUtil.View;

public partial class FileMergeSplitView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly Type[] Routers = {
        typeof(FileMergeView),
        typeof(FileSplitView),
    };

    public FileMergeSplitView() {
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
