using CommonUITools.Route;
using CommonUITools.Utils;
using CommonUtil.Store;
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

public partial class MainWindow : Window {
    public static readonly DependencyProperty IsBackIconVisibleProperty = DependencyProperty.Register("IsBackIconVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
    public static readonly DependencyProperty RouteViewTitleProperty = DependencyProperty.Register("RouteViewTitle", typeof(string), typeof(MainWindow), new PropertyMetadata(Global.AppTitle));
    public static readonly DependencyProperty ShowLoadingBoxProperty = DependencyProperty.Register("ShowLoadingBox", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

    /// <summary>
    /// 返回Icon是否可见
    /// </summary>
    public bool IsBackIconVisible {
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
    public bool ShowLoadingBox {
        get { return (bool)GetValue(ShowLoadingBoxProperty); }
        set { SetValue(ShowLoadingBoxProperty, value); }
    }
    /// <summary>
    /// 标题动画
    /// </summary>
    private readonly Storyboard TitleBarStoryboard;
    /// <summary>
    /// 背景颜色动画
    /// </summary>
    private readonly Storyboard MainContentViewLoadStoryboard;
    private readonly DoubleAnimation TranslateTransformXAnimation;
    private readonly ColorAnimation MainContentViewBackgroundAnimation;

    public MainWindow() {
        InitializeComponent();
        #region 设置 Storyboard
        TitleBarStoryboard = (Storyboard)Resources["TitleBarStoryboard"];
        MainContentViewLoadStoryboard = (Storyboard)Resources["MainContentViewLoadStoryboard"];
        TranslateTransformXAnimation = (DoubleAnimation)TitleBarStoryboard.Children.First(t => t.Name == "TranslateTransformX");
        MainContentViewBackgroundAnimation = (ColorAnimation)MainContentViewLoadStoryboard.Children.First(t => t.Name == "BackgroundAnimation");
        #endregion
        CommonUITools.App.RegisterWidgetPage(this);
        _ = new MainWindowRouter(ContentFrame);
        // 延迟加载，减少卡顿
        Loaded += (_, _) => Task.Run(() => {
            Thread.Sleep(1000);
            Dispatcher.BeginInvoke(() => MainWindowRouter.Navigate(typeof(MainContentView)));
        });
        // 设置 AppTheme
        //ThemeManager.Current.AccentColor = ((SolidColorBrush)Global.ThemeResource["ApplicationAccentColor"]).Color;
    }

    /// <summary>
    /// navigation 改变事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContentFrameNavigatedHandler(object sender, NavigationEventArgs e) {
        Type contentType = e.Content.GetType();
        IsBackIconVisible = contentType != typeof(MainContentView);
        // 主窗口菜单列表
        if (contentType == typeof(MainContentView)) {
            RouteViewTitle = Global.AppTitle;
            MainContentViewBackgroundAnimation.To = ((SolidColorBrush)FindResource("MainContentViewBackground")).Color;
            TranslateTransformXAnimation.From = -100;
        } else {
            MainContentViewBackgroundAnimation.To = Colors.White;
            foreach (var item in Global.MenuItems) {
                if (item.ClassType == contentType) {
                    RouteViewTitle = item.Name;
                    break;
                }
            }
            TranslateTransformXAnimation.From = 100;
        }
        TitleBarStoryboard.Begin();
        MainContentViewLoadStoryboard.Begin();
        ShowLoadingBox = false;
    }

    private void ToBackClick(object sender, RoutedEventArgs e) {
        MainWindowRouter.Back();
    }

}

