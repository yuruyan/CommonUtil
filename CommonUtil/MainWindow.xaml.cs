using CommonUITools.Widget;
using CommonUtil.View;
using System.Windows.Navigation;

namespace CommonUtil;

[SingleInstance]
public partial class MainWindow : BaseWindow {
    public static readonly DependencyProperty RouteViewTitleProperty = DependencyProperty.Register("RouteViewTitle", typeof(string), typeof(MainWindow), new PropertyMetadata(Global.AppTitle));
    private static readonly DependencyProperty ShowLoadingBoxProperty = DependencyProperty.Register("ShowLoadingBox", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));
    private static readonly DependencyProperty CurrentThemeModeProperty = DependencyProperty.Register("CurrentThemeMode", typeof(ThemeMode), typeof(MainWindow), new PropertyMetadata(ThemeMode.Light));
    public static readonly DependencyProperty PreviousBackgroundColorProperty = DependencyProperty.Register("PreviousBackgroundColor", typeof(Color), typeof(MainWindow), new PropertyMetadata(Colors.Transparent));
    public static readonly DependencyProperty CurrentBackgroundColorProperty = DependencyProperty.Register("CurrentBackgroundColor", typeof(Color), typeof(MainWindow), new PropertyMetadata(Colors.Transparent));
    public static readonly DependencyProperty IsNavigationButtonVisibleProperty = DependencyProperty.Register("IsNavigationButtonVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
    public static readonly DependencyProperty IsHomeButtonVisibleProperty = DependencyProperty.Register("IsHomeButtonVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

    /// <summary>
    /// 导航按钮是否可见
    /// </summary>
    public bool IsNavigationButtonVisible {
        get { return (bool)GetValue(IsNavigationButtonVisibleProperty); }
        set { SetValue(IsNavigationButtonVisibleProperty, value); }
    }
    /// <summary>
    /// 主页按钮是否可见
    /// </summary>
    public bool IsHomeButtonVisible {
        get { return (bool)GetValue(IsHomeButtonVisibleProperty); }
        set { SetValue(IsHomeButtonVisibleProperty, value); }
    }
    /// <summary>
    /// 标题
    /// </summary>
    public string RouteViewTitle {
        get { return (string)GetValue(RouteViewTitleProperty); }
        set { SetValue(RouteViewTitleProperty, value); }
    }
    /// <summary>
    /// 显示加载框
    /// </summary>
    private bool ShowLoadingBox {
        get { return (bool)GetValue(ShowLoadingBoxProperty); }
        set { SetValue(ShowLoadingBoxProperty, value); }
    }
    /// <summary>
    /// 当前 ThemeMode
    /// </summary>
    private ThemeMode CurrentThemeMode {
        get { return (ThemeMode)GetValue(CurrentThemeModeProperty); }
        set { SetValue(CurrentThemeModeProperty, value); }
    }
    /// <summary>
    /// 上一个 WindowBackgroundColor
    /// </summary>
    public Color PreviousBackgroundColor {
        get { return (Color)GetValue(PreviousBackgroundColorProperty); }
        set { SetValue(PreviousBackgroundColorProperty, value); }
    }
    /// <summary>
    /// 当前 WindowBackgroundColor
    /// </summary>
    public Color CurrentBackgroundColor {
        get { return (Color)GetValue(CurrentBackgroundColorProperty); }
        set { SetValue(CurrentBackgroundColorProperty, value); }
    }
    private readonly Storyboard MainWindowBackgroundStoryboard;
    private readonly RouterService RouterService;
    private bool IsNavigationContentViewInitialized;

    public MainWindow() {
        InitializeComponent();
        RouterService = InitRouterService();
        MainWindowBackgroundStoryboard = (Storyboard)Resources["MainWindowBackgroundStoryboard"];
        AutoHideHelper.SetOpenOnClick(NavigationButton, ElementVisibilityHelperNames.NavigationContentListView);

        // Navigate to MainContentView
        this.SetLoadedOnceEventHandler(async (_, _) => {
            // 延迟加载，减少卡顿
            await Task.Delay(1000);
            RouterService.Navigate(typeof(MainContentView));
            ShowLoadingBox = false;
        });
        // ThemeChanged
        ThemeManager.Current.ThemeChanged += (_, mode) => {
            CurrentThemeMode = mode;
            PreviousBackgroundColor = ((SolidColorBrush)Background).Color;
            CurrentBackgroundColor = ((SolidColorBrush)FindResource("WindowBackgroundBrush")).Color;
            MainWindowBackgroundStoryboard.Begin();
        };
    }

    private RouterService InitRouterService() {
        return new RouterService(
            ContentFrame,
            new Type[] {
                typeof(MainContentView),
                typeof(NavigationContentView)
            }
        );
    }

    /// <summary>
    /// navigation 改变事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContentFrameNavigatedHandler(object sender, NavigationEventArgs e) {
        // Show NavigationButton
        if (e.Content is NavigationContentView contentView) {
            IsNavigationButtonVisible = true;
            // Set IsNavigationButtonVisible
            if (!IsNavigationContentViewInitialized) {
                var menuItems = contentView.ToolMenuItems;
                menuItems.CollectionChanged += (sender, args) => {
                    IsNavigationButtonVisible = menuItems.Count != 0;
                };
            }
            IsNavigationContentViewInitialized = true;
        }
        // Update IsHomeButtonVisible
        IsHomeButtonVisible = e.Content is not MainContentView;
        if (e.Content is INavigationRequest<NavigationRequestArgs> navigator) {
            navigator.NavigationRequested -= ContentNavigationRequested;
            navigator.NavigationRequested += ContentNavigationRequested;
        }
    }

    /// <summary>
    /// 导航请求
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContentNavigationRequested(object? sender, NavigationRequestArgs e) {
        RouterService.Navigate(e.ViewType, e.Data);
    }

    /// <summary>
    /// 切换为 LightTheme
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SwitchToLightThemeClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        ThrottleUtils.Throttle(
            ThemeManager.Current,
            () => ThemeManager.Current.SwitchToLightTheme(),
            1000
        );
    }

    /// <summary>
    /// 切换为 DarkTheme
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SwitchToDarkThemeClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        ThrottleUtils.Throttle(
            ThemeManager.Current,
            () => ThemeManager.Current.SwitchToDarkTheme(),
            1000
        );
    }

    /// <summary>
    /// 导航到主页
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NavigateToMainContentViewClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        RouterService.Navigate(typeof(MainContentView));
    }

    /// <summary>
    /// 导航到 NavigationContentView
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NavigateToNavigationContentViewClickHandler(object sender, RoutedEventArgs e) {
        // Do not set 'e.Handled = true'
        RouterService.Navigate(typeof(NavigationContentView));
    }
}
