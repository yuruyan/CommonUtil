using System.Windows.Navigation;
using NavigationView = ModernWpf.Controls.NavigationView;

namespace CommonUtil.Route;

internal static class NavigationUtils {
    private const string NavigationViewMaxWidthKey = "NavigationViewMaxWidth";
    private const string AnimationDurationKey = "AnimationDuration";
    private const string AnimationEaseFunctionKey = "AnimationEaseFunction";
    private const string OpenPaneLengthProperty = "OpenPaneLength";

    private readonly struct NavigationViewInfo {
        public double InitialOpenPaneLength { get; }
        public Storyboard ExpandStoryboard { get; }
        public Storyboard ShrinkStoryboard { get; }

        public NavigationViewInfo(double initialOpenPaneLength, Storyboard expandStoryboard, Storyboard shrinkStoryboard) {
            InitialOpenPaneLength = initialOpenPaneLength;
            ExpandStoryboard = expandStoryboard;
            ShrinkStoryboard = shrinkStoryboard;
        }
    }

    private static readonly IDictionary<NavigationView, NavigationViewInfo> NavigationViewInfoDict = new Dictionary<NavigationView, NavigationViewInfo>();
    private static readonly IDictionary<Window, WindowState> WindowPreviousStateDict = new Dictionary<Window, WindowState>();
    private static readonly IDictionary<Window, object> WindowKeyDict = new Dictionary<Window, object>();

    private static readonly IDictionary<NavigationView, RouterService> NavigationViewRouterServiceDict = new Dictionary<NavigationView, RouterService>();
    private static readonly IDictionary<Frame, NavigationView> FrameNavigationViewDict = new Dictionary<Frame, NavigationView>();

    /// <summary>
    /// 启用导航
    /// </summary>
    /// <param name="navigationView"></param>
    /// <param name="routerService"></param>
    /// <param name="frame"></param>
    /// <remarks>
    /// 需要在 xaml 里设置 NavigationItem，并且 Name 属性设置为 PageType.Name 属性
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
    /// Duration: 静态资源为 AnimationDuration <br/>
    /// ExpandOpenPaneLength：静态资源为 NavigationViewMaxWidth <br/>
    /// EasingFunction：静态资源为 AnimationEaseFunction <br/>
    /// </remarks>
    public static void EnableNavigationPanelResponsive(NavigationView navigationView) {
        TaskUtils.EnsureCalledOnce(navigationView, () => {
            if (navigationView.IsLoaded) {
                EnableNavigationPanelResponsiveInternal(navigationView);
            }
            // 等待加载
            navigationView.Loaded += NavigationViewLoadedHandler;
        });
    }

    private static void NavigationViewLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is NavigationView navigationView) {
            navigationView.Loaded -= NavigationViewLoadedHandler;
            EnableNavigationPanelResponsiveInternal(navigationView);
        }
    }

    /// <summary>
    /// IsLoaded 时加载
    /// </summary>
    /// <param name="navigationView"></param>
    private static void EnableNavigationPanelResponsiveInternal(NavigationView navigationView) {
        NavigationViewInfoDict[navigationView] = new(
            navigationView.OpenPaneLength,
            GetNavigationViewExpandStoryboard(navigationView),
            GetNavigationViewShrinkStoryboard(navigationView)
        );
        var window = Window.GetWindow(navigationView);
        #region 设置 Dict
        if (!WindowKeyDict.ContainsKey(window)) {
            WindowKeyDict[window] = new object();
        }
        WindowPreviousStateDict[window] = window.WindowState;
        #endregion
        BeginStoryBoard(window, new NavigationView[] { navigationView });
        // 每个Window执行一次，同时执行所有动画
        TaskUtils.EnsureCalledOnce(WindowKeyDict[window], () => {
            DependencyPropertyDescriptor
                .FromProperty(Window.WindowStateProperty, typeof(Window))
                .AddValueChanged(window, (_, _) => {
                    // 从最小化变化而来则不执行
                    if (WindowPreviousStateDict[window] != WindowState.Minimized) {
                        BeginStoryBoard(window, NavigationViewInfoDict.Keys);
                    }
                    WindowPreviousStateDict[window] = window.WindowState;
                });
        });
    }

    /// <summary>
    /// 开始动画
    /// </summary>
    /// <param name="window"></param>
    /// <param name="navigationViews"></param>
    private static void BeginStoryBoard(Window window, IEnumerable<NavigationView> navigationViews) {
        // 展开
        if (window.WindowState == WindowState.Maximized) {
            // 并行
            foreach (var item in navigationViews) {
                // 为了减少卡顿
                _ = TaskUtils.DelayTaskAsync(
                    200,
                    () => UIUtils.RunOnUIThread(() => NavigationViewInfoDict[item].ExpandStoryboard.Begin())
                );
            }
        }
        // 收缩
        else if (window.WindowState == WindowState.Normal) {
            // 并行
            foreach (var item in navigationViews) {
                // 为了减少卡顿
                _ = TaskUtils.DelayTaskAsync(
                    200,
                    () => UIUtils.RunOnUIThread(() => NavigationViewInfoDict[item].ShrinkStoryboard.Begin())
                );
            }
        }
    }

    private static Storyboard GetNavigationViewExpandStoryboard(NavigationView navigationView) {
        var animation = new DoubleAnimation(
            navigationView.OpenPaneLength,
            (double)navigationView.FindResource(NavigationViewMaxWidthKey),
            (Duration)navigationView.FindResource(AnimationDurationKey)
        ) {
            EasingFunction = (IEasingFunction)navigationView.FindResource(AnimationEaseFunctionKey)
        };
        Storyboard.SetTarget(animation, navigationView);
        Storyboard.SetTargetProperty(animation, new(OpenPaneLengthProperty));
        return new() { Children = { animation } };
    }

    private static Storyboard GetNavigationViewShrinkStoryboard(NavigationView navigationView) {
        var animation = new DoubleAnimation(
            (double)navigationView.FindResource(NavigationViewMaxWidthKey),
            navigationView.OpenPaneLength,
            (Duration)navigationView.FindResource(AnimationDurationKey)
        ) {
            EasingFunction = (IEasingFunction)navigationView.FindResource(AnimationEaseFunctionKey)
        };
        Storyboard.SetTarget(animation, navigationView);
        Storyboard.SetTargetProperty(animation, new(OpenPaneLengthProperty));
        return new() { Children = { animation } };
    }
}
