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

    public MainWindow() {
        InitializeComponent();
        RouterService = InitRouterService();
        MainWindowBackgroundStoryboard = (Storyboard)Resources["MainWindowBackgroundStoryboard"];

        // 导航到 MainContentView
        Loaded += async (_, _) => {
            // 延迟加载，减少卡顿
            await Task.Delay(1000);
            RouterService.Navigate(typeof(MainContentView));
            ShowLoadingBox = false;
        };
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
            Global.MenuItems.Select(t => t.ClassType).Join(new Type[] {
                typeof(MainContentView)
            })
        );
    }

    /// <summary>
    /// navigation 改变事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContentFrameNavigatedHandler(object sender, NavigationEventArgs e) {
        //Type contentType = e.Content.GetType();
        //// 修改 title
        //if (contentType == typeof(MainContentView)) {
        //    RouteViewTitle = Global.AppTitle;
        //} else {
        //    RouteViewTitle = Global.MenuItems.First(t => t.ClassType == e.Content.GetType()).Name;
        //}
        //ShowLoadingBox = false;
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
}
