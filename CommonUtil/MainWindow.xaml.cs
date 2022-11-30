using CommonUITools.Model;
using CommonUITools.Route;
using CommonUITools.Utils;
using CommonUtil.Store;
using CommonUtil.Theme;
using CommonUtil.View;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace CommonUtil;

[SingleInstance]
public partial class MainWindow : Window {
    public static readonly DependencyProperty RouteViewTitleProperty = DependencyProperty.Register("RouteViewTitle", typeof(string), typeof(MainWindow), new PropertyMetadata(Global.AppTitle));
    public static readonly DependencyProperty TitleBarBackgroundProperty = DependencyProperty.Register("TitleBarBackground", typeof(Brush), typeof(MainWindow), new PropertyMetadata());
    private static readonly DependencyProperty IsBackIconVisibleProperty = DependencyProperty.Register("IsBackIconVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
    private static readonly DependencyProperty ShowLoadingBoxProperty = DependencyProperty.Register("ShowLoadingBox", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));
    private static readonly DependencyProperty CurrentThemeModeProperty = DependencyProperty.Register("CurrentThemeMode", typeof(ThemeMode), typeof(MainWindow), new PropertyMetadata(ThemeMode.Light));
    public static readonly DependencyProperty PreviousBackgroundColorProperty = DependencyProperty.Register("PreviousBackgroundColor", typeof(Color), typeof(MainWindow), new PropertyMetadata(Colors.Transparent));
    public static readonly DependencyProperty CurrentBackgroundColorProperty = DependencyProperty.Register("CurrentBackgroundColor", typeof(Color), typeof(MainWindow), new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// 返回Icon是否可见
    /// </summary>
    private bool IsBackIconVisible {
        get { return (bool)GetValue(IsBackIconVisibleProperty); }
        set { SetValue(IsBackIconVisibleProperty, value); }
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
    /// 标题栏背景
    /// </summary>
    public Brush TitleBarBackground {
        get { return (Brush)GetValue(TitleBarBackgroundProperty); }
        set { SetValue(TitleBarBackgroundProperty, value); }
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
    /// <summary>
    /// 标题动画
    /// </summary>
    private readonly Storyboard TitleBarStoryboard;
    /// <summary>
    /// 背景颜色动画
    /// </summary>
    private readonly Storyboard MainContentViewLoadStoryboard;
    private readonly Storyboard MainWindowBackgroundStoryboard;
    private readonly DoubleAnimation TranslateTransformXAnimation;
    private readonly ColorAnimation MainContentViewBackgroundAnimation;
    private readonly RouterService RouterService;

    public MainWindow() {
        InitializeComponent();
        #region RouterService
        var pageTypes = Global.MenuItems.Select(t => t.ClassType).ToList();
        pageTypes.Add(typeof(MainContentView));
        RouterService = new(ContentFrame, pageTypes);
        MainWindowRouter.AddPageFrame(pageTypes, RouterService);
        MainWindowRouter.RouteChanged += (_, _) => {
            IsBackIconVisible = MainWindowRouter.CanGoBack;
        };
        #endregion
        #region 设置 Storyboard
        TitleBarStoryboard = (Storyboard)Resources["TitleBarStoryboard"];
        MainWindowBackgroundStoryboard = (Storyboard)Resources["MainWindowBackgroundStoryboard"];
        MainContentViewLoadStoryboard = (Storyboard)Resources["MainContentViewLoadStoryboard"];
        TranslateTransformXAnimation = (DoubleAnimation)TitleBarStoryboard.Children.First(t => t.Name == "TranslateTransformX");
        MainContentViewBackgroundAnimation = (ColorAnimation)MainContentViewLoadStoryboard.Children.First(t => t.Name == "BackgroundAnimation");
        #endregion 
        CommonUITools.App.RegisterWidgetPage(this);
        // 导航到 MainContentView
        Loaded += (_, _) => Task.Run(() => {
            // 延迟加载，减少卡顿
            Thread.Sleep(1000);
            Dispatcher.BeginInvoke(() => {
                RouterService.Navigate(typeof(MainContentView));
                MainWindowRouter.PushRouteStack(ContentFrame);
            });
        });
        ThemeManager.Current.ThemeChanged += (_, mode) => {
            CurrentThemeMode = mode;
            PreviousBackgroundColor = ((SolidColorBrush)Background).Color;
            CurrentBackgroundColor = ((SolidColorBrush)FindResource("WindowBackgroundBrush")).Color;
            MainWindowBackgroundStoryboard.Begin();
        };
    }

    /// <summary>
    /// navigation 改变事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContentFrameNavigatedHandler(object sender, NavigationEventArgs e) {
        Type contentType = e.Content.GetType();
        // 修改 title
        if (contentType == typeof(MainContentView)) {
            RouteViewTitle = Global.AppTitle;
        } else {
            RouteViewTitle = Global.MenuItems.First(t => t.ClassType == e.Content.GetType()).Name;
        }
        ShowLoadingBox = false;
    }

    /// <summary>
    /// 回退
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToBackClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        MainWindowRouter.GoBack();
    }

    /// <summary>
    /// 回到主页
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToMainPageClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        RouterService.Navigate(typeof(MainContentView));
        MainWindowRouter.PushRouteStack(ContentFrame);
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
