using CommonUITools.Route;
using CommonUtil.Route;
using NLog;
using System;

namespace CommonUtil.View;

public partial class TextToolView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly Type[] Routers = {
        typeof(RemoveDuplicateView),
        typeof(WhiteSpaceProcessView),
        typeof(EnglishTextProcessView),
        typeof(PrependLineNumberView),
        typeof(AddEnglishWordBraces),
    };
    private readonly RouterService RouterService;

    public TextToolView() {
        InitializeComponent();
        RouterService = new(ContentFrame, Routers);
        NavigationUtils.EnableNavigation(NavigationView, RouterService, ContentFrame);
    }
}
