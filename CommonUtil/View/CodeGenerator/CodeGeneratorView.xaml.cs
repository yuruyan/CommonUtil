using CommonUtil.Route;

namespace CommonUtil.View;

public partial class CodeGeneratorView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly Type[] Routers = {
        typeof(CSharpDependencyView),
    };

    public CodeGeneratorView() {
        InitializeComponent();
        NavigationUtils.EnableNavigation(
            NavigationView,
            new(ContentFrame, Routers),
            ContentFrame
        );
    }
}
