
namespace CommonUtil.View;

public partial class CodeGeneratorView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly RouterService RouterService;
    private readonly Type[] Routers = {
        typeof(CSharpDependencyView),
    };

    public CodeGeneratorView() {
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
