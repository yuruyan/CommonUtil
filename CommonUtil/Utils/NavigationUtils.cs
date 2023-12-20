using ModernWpf.Controls;
using System.Windows.Navigation;
using Frame = System.Windows.Controls.Frame;

namespace CommonUtil.Utils;

internal static class NavigationUtils {
    private const string NavigationViewOpenPaneDefaultWidthKey = "NavigationViewOpenPaneDefaultWidth";
    private const string NavigationViewOpenPaneExpansionWidthKey = "NavigationViewOpenPaneExpansionWidth";
    private const string AnimationDurationKey = "AnimationDuration";
    private const string AnimationEaseFunctionKey = "AnimationEaseFunction";
    private const string OpenPaneLengthProperty = "OpenPaneLength";
    private const string NavigationViewExpansionThresholdWidthKey = "NavigationViewExpansionThresholdWidth";
    /// <summary>
    /// Delay handle responsive layout
    /// </summary>
    private const int HandleResponsiveDelayDuration = 250;

    private static readonly IDictionary<NavigationView, RouterService> NavigationViewRouterServiceDict = new Dictionary<NavigationView, RouterService>();
    private static readonly IDictionary<Frame, NavigationView> FrameNavigationViewDict = new Dictionary<Frame, NavigationView>();
    private static readonly IDictionary<NavigationView, NavigationViewInfo> NavigationViewInfoDict = new Dictionary<NavigationView, NavigationViewInfo>();

    private struct NavigationViewInfo {
        public readonly double InitialOpenPaneLength { get; }
        public readonly double ExpansionThresholdWidth { get; }
        public readonly Storyboard ExpandStoryboard { get; }
        public readonly Storyboard ShrinkStoryboard { get; }
        public bool IsExpanded { get; set; }

        public NavigationViewInfo(double initialOpenPaneLength, double expansionThresholdWidth, Storyboard expandStoryboard, Storyboard shrinkStoryboard) {
            InitialOpenPaneLength = initialOpenPaneLength;
            ExpansionThresholdWidth = expansionThresholdWidth;
            ExpandStoryboard = expandStoryboard;
            ShrinkStoryboard = shrinkStoryboard;
        }
    }

    /// <summary>
    /// 初始化 NavigationViewItems
    /// </summary>
    /// <param name="navigationView"></param>
    /// <param name="navigationItems"></param>
    public static void InitializeNavigationViewItems(NavigationView navigationView, IEnumerable<NavigationItemInfo> navigationItems) {
        foreach (var item in navigationItems) {
            IconElement icon = item.IconImage is null
                ? new FontIcon() { Glyph = item.Icon }
                : new BitmapIcon() { ShowAsMonochrome = item.ShowAsMonochrome, UriSource = item.IconImage };
            // Set icon color
            if (item.IconColor is string color && icon is FontIcon fontIcon) {
                fontIcon.Foreground = color.ToBrush();
            }
            navigationView.MenuItems.Add(new NavigationViewItem() {
                Name = item.Name,
                Content = item.Content,
                ToolTip = new ToolTip() { Content = item.ToolTip ?? item.Content },
                Icon = icon,
            });
        }
    }

    /// <summary>
    /// 启用导航
    /// </summary>
    /// <param name="navigationView"></param>
    /// <param name="routerService"></param>
    /// <param name="frame"></param>
    /// <remarks>
    /// 需要在 xaml 里设置 NavigationItem，并且 Name 属性设置为 PageType.Name 属性 <br/>
    /// 默认选中第一项
    /// </remarks>
    public static void EnableNavigation(
        NavigationView navigationView,
        RouterService routerService,
        Frame frame
    ) {
        NavigationViewRouterServiceDict[navigationView] = routerService;
        FrameNavigationViewDict[frame] = navigationView;

        // 主动导航跳转
        navigationView.SelectionChanged -= NavigationViewSelectionChangedHandler;
        navigationView.SelectionChanged += NavigationViewSelectionChangedHandler;
        // 默认选择第一项
        if (navigationView.SelectedItem is null && navigationView.MenuItems.Count > 0) {
            navigationView.SelectedItem = navigationView.MenuItems[0];
        }
        // 自动导航跳转
        frame.Navigated -= FrameNavigatedHandler;
        frame.Navigated += FrameNavigatedHandler;
    }

    private static void NavigationViewSelectionChangedHandler(NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args) {
        if (args.SelectedItem is FrameworkElement element) {
            var routerService = NavigationViewRouterServiceDict[sender];
            routerService.Navigate(
                routerService.RouteTypes.First(t => t.Name == element.Name)
            );
        }
    }

    private static void FrameNavigatedHandler(object sender, NavigationEventArgs e) {
        if (sender is Frame frame) {
            var navigationView = FrameNavigationViewDict[frame];
            navigationView.SelectedItem = navigationView.MenuItems
                .Cast<FrameworkElement>()
                .FirstOrDefault(item => item.Name == e.Content.GetType().Name);
        }
    }

    /// <summary>
    /// 禁用导航，移除引用
    /// </summary>
    /// <param name="navigationView"></param>
    public static void DisableNavigation(NavigationView navigationView) {
        NavigationViewRouterServiceDict.Remove(navigationView);
        navigationView.SelectionChanged -= NavigationViewSelectionChangedHandler;

        var targetFrame = FrameNavigationViewDict.FirstOrDefault(kv => kv.Value == navigationView);
        if (!object.Equals(targetFrame, default(KeyValuePair<Frame, NavigationView>))) {
            targetFrame.Key.Navigated -= FrameNavigatedHandler;
            FrameNavigationViewDict.Remove(targetFrame);
        }
    }

    /// <summary>
    /// 启用响应式 NavigationView 布局
    /// </summary>
    /// <param name="navigationView"></param>
    /// <remarks>
    /// OpenPaneExpansionWidth：资源为 <see cref="NavigationViewOpenPaneExpansionWidthKey"/> <br/>
    /// OpenPaneDefaultWidth：资源为 <see cref="NavigationViewOpenPaneDefaultWidthKey"/> <br/>
    /// </remarks>
    public static void EnableNavigationPanelResponsive(NavigationView navigationView) {
        if (navigationView.IsLoaded) {
            EnableNavigationPanelResponsiveInternal(navigationView);
            return;
        }
        // 等待加载
        navigationView.Loaded += NavigationViewLoadedHandler;
    }

    /// <summary>
    /// 禁用响应式布局，移除引用
    /// </summary>
    /// <param name="navigationView"></param>
    public static void DisableNavigationPanelResponsive(NavigationView navigationView) {
        navigationView.Loaded -= NavigationViewLoadedHandler;
        navigationView.SizeChanged -= NavigationViewSizeChangedHandler;
        NavigationViewInfoDict.Remove(navigationView);
    }

    private static void NavigationViewLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is NavigationView navigationView) {
            navigationView.Loaded -= NavigationViewLoadedHandler;
            EnableNavigationPanelResponsiveInternal(navigationView);
        }
    }

    private static void EnableNavigationPanelResponsiveInternal(NavigationView navigationView) {
        double openPaneDefaultLength = (double)navigationView.FindResource(NavigationViewOpenPaneDefaultWidthKey);
        double openPaneExpansionLength = (double)navigationView.FindResource(NavigationViewOpenPaneExpansionWidthKey);
        Duration duration = (Duration)navigationView.FindResource(AnimationDurationKey);
        IEasingFunction easingFunction = (IEasingFunction)navigationView.FindResource(AnimationEaseFunctionKey);

        NavigationViewInfoDict[navigationView] = new(
            openPaneDefaultLength,
            (double)navigationView.FindResource(NavigationViewExpansionThresholdWidthKey),
            GetNavigationViewExpandStoryboard(navigationView, openPaneDefaultLength, openPaneExpansionLength, duration, easingFunction),
            GetNavigationViewShrinkStoryboard(navigationView, openPaneDefaultLength, openPaneExpansionLength, duration, easingFunction)
        );
        navigationView.SizeChanged -= NavigationViewSizeChangedHandler;
        navigationView.SizeChanged += NavigationViewSizeChangedHandler;
        // Explicitly invoke
        HandleNavigationViewSizeChanged(navigationView, navigationView.ActualWidth, true);
    }

    private static void NavigationViewSizeChangedHandler(object sender, SizeChangedEventArgs e) {
        if (sender is NavigationView view) {
            HandleNavigationViewSizeChanged(view, e.NewSize.Width);
        }
    }

    /// <summary>
    /// Handle SizeChanged event
    /// </summary>
    /// <param name="view"></param>
    /// <param name="newWidth"></param>
    /// <param name="isInitial">是否初次调用</param>
    private static async void HandleNavigationViewSizeChanged(NavigationView view, double newWidth, bool isInitial = false) {
        // Delay
        if (!isInitial) {
            await Task.Delay(HandleResponsiveDelayDuration);
        }
        if (!NavigationViewInfoDict.TryGetValue(view, out var info)) {
            return;
        }

        if (newWidth >= info.ExpansionThresholdWidth && (isInitial || !info.IsExpanded)) {
            info.IsExpanded = true;
            info.ExpandStoryboard.Begin();
        } else if (newWidth < info.ExpansionThresholdWidth && (isInitial || info.IsExpanded)) {
            info.IsExpanded = false;
            info.ShrinkStoryboard.Begin();
        }
        NavigationViewInfoDict[view] = info;
    }

    private static Storyboard GetNavigationViewExpandStoryboard(
        NavigationView navigationView,
        double openPaneDefaultLength,
        double openPaneExpansionLength,
        Duration duration,
        IEasingFunction easingFunction
    ) {
        var animation = new DoubleAnimation(
            openPaneDefaultLength,
            openPaneExpansionLength,
            duration
        ) { EasingFunction = easingFunction };
        Storyboard.SetTarget(animation, navigationView);
        Storyboard.SetTargetProperty(animation, new(OpenPaneLengthProperty));
        return new() { Children = { animation } };
    }

    private static Storyboard GetNavigationViewShrinkStoryboard(
        NavigationView navigationView,
        double openPaneDefaultLength,
        double openPaneExpansionLength,
        Duration duration,
        IEasingFunction easingFunction
    ) {
        var animation = new DoubleAnimation(
            openPaneExpansionLength,
            openPaneDefaultLength,
            duration
        ) { EasingFunction = easingFunction };
        Storyboard.SetTarget(animation, navigationView);
        Storyboard.SetTargetProperty(animation, new(OpenPaneLengthProperty));
        return new() { Children = { animation } };
    }
}
