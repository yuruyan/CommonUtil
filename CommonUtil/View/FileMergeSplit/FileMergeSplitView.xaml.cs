using CommonUtil.Route;

namespace CommonUtil.View;

public partial class FileMergeSplitView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly Type[] Routers = {
        typeof(FileMergeView),
        typeof(FileSplitView),
    };

    public FileMergeSplitView() {
        InitializeComponent();
        NavigationUtils.EnableNavigation(
            NavigationView,
            new(ContentFrame, Routers),
            ContentFrame
        );
    }
}
