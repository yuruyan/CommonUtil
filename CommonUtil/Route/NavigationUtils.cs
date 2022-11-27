using CommonUITools.Route;
using CommonUITools.Utils;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace CommonUtil.Route;

internal static class NavigationUtils {
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
        // 主动导航跳转
        navigationView.SelectionChanged += (s, e) => {
            if (e.SelectedItem is FrameworkElement element) {
                routerService.Navigate(routerService.GetRouteTypes().First(t => t.Name == element.Name));
                MainWindow.PushRouteStack(routerService);
            }
        };
        // 自动导航跳转
        frame.Navigated += (s, e) => {
            navigationView.SelectedItem = navigationView.MenuItems
                .Cast<FrameworkElement>()
                .FirstOrDefault(item => item.Name == e.Content.GetType().Name);
        };
    }

    private readonly struct NavigationViewInfo {
        public readonly double InitialOpenPaneLength { get; }
        public readonly Storyboard ExpandStoryboard { get; }
        public readonly Storyboard ShrinkStoryboard { get; }

        public NavigationViewInfo(double initialOpenPaneLength, Storyboard expandStoryboard, Storyboard shrinkStoryboard) {
            InitialOpenPaneLength = initialOpenPaneLength;
            ExpandStoryboard = expandStoryboard;
            ShrinkStoryboard = shrinkStoryboard;
        }
    }

    private static readonly IDictionary<NavigationView, NavigationViewInfo> NavigationViewInfoDict = new Dictionary<NavigationView, NavigationViewInfo>();

    private static readonly IDictionary<Window, WindowState> WindowPreviousStateDict = new Dictionary<Window, WindowState>();

    private static readonly IDictionary<Window, object> WindowKeyDict = new Dictionary<Window, object>();

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
                    () => App.Current.Dispatcher.Invoke(() => NavigationViewInfoDict[item].ExpandStoryboard.Begin())
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
                    () => App.Current.Dispatcher.Invoke(() => NavigationViewInfoDict[item].ShrinkStoryboard.Begin())
                );
            }
        }
    }

    private static Storyboard GetNavigationViewExpandStoryboard(NavigationView navigationView) {
        var animation = new DoubleAnimation(
            navigationView.OpenPaneLength,
            (double)navigationView.FindResource("NavigationViewMaxWidth"),
            (Duration)navigationView.FindResource("AnimationDuration")
        ) {
            EasingFunction = (IEasingFunction)navigationView.FindResource("AnimationEaseFunction")
        };
        Storyboard.SetTarget(animation, navigationView);
        Storyboard.SetTargetProperty(animation, new("OpenPaneLength"));
        return new() { Children = { animation } };
    }

    private static Storyboard GetNavigationViewShrinkStoryboard(NavigationView navigationView) {
        var animation = new DoubleAnimation(
            (double)navigationView.FindResource("NavigationViewMaxWidth"),
            navigationView.OpenPaneLength,
            (Duration)navigationView.FindResource("AnimationDuration")
        ) {
            EasingFunction = (IEasingFunction)navigationView.FindResource("AnimationEaseFunction")
        };
        Storyboard.SetTarget(animation, navigationView);
        Storyboard.SetTargetProperty(animation, new("OpenPaneLength"));
        return new() { Children = { animation } };
    }
}
