﻿namespace CommonUtil.View;

public partial class TextToolView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly Type[] Routers = {
        typeof(RemoveDuplicateView),
        typeof(WhiteSpaceProcessView),
        typeof(EnglishTextProcessView),
        typeof(PrependLineNumberView),
        typeof(AddEnglishWordBraces),
        typeof(InvertTextView),
        typeof(SortLinesView),
    };

    public TextToolView() {
        InitializeComponent();
        NavigationUtils.EnableNavigation(
            NavigationView,
            new(ContentFrame, Routers),
            ContentFrame
        );
        NavigationUtils.EnableNavigationPanelResponsive(NavigationView);
    }
}
