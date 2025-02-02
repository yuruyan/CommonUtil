﻿namespace CommonUtil.View;

public partial class TextToolView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly RouterService RouterService;
    private readonly Type[] Routers = {
        typeof(RemoveDuplicateView),
        typeof(WhiteSpaceProcessView),
        typeof(EnglishTextProcessView),
        typeof(PrependLineNumberView),
        typeof(EnglishWordBracesView),
        typeof(InvertTextView),
        typeof(SortLinesView),
        typeof(ShuffleLinesView),
        typeof(PunctuationReplacementView),
        typeof(MarkdownTableConversionView),
        typeof(HtmlTableConversionView),
        typeof(LengthPartitionView),
        typeof(CrossJoinView),
        typeof(DictionaryReplacementView),
    };

    public TextToolView() {
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
