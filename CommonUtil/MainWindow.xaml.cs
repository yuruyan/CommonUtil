using CommonUITools.Route;
using CommonUtil.Store;
using CommonUtil.View;
using ModernWpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace CommonUtil {
    public partial class MainWindow : Window {
        public static readonly DependencyProperty IsBackIconVisibleProperty = DependencyProperty.Register("IsBackIconVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty RouteViewTitleProperty = DependencyProperty.Register("RouteViewTitle", typeof(string), typeof(MainWindow), new PropertyMetadata(Global.AppTitle));

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

            _ = new MainWindowRouter(ContentFrame);
            ContentFrame.Navigated += ContentFrameNavigatedHandler; // navigation 改变事件
            CommonUITools.Widget.MessageBox.PanelChildren = MessageBoxPanel.Children;  // 初始化
            CommonUITools.Widget.NotificationBox.PanelChildren = NotificationPanel.Children;  // 初始化
            MainWindowRouter.Navigate(typeof(MainContentView));
            // 设置 AppTheme
            ThemeManager.Current.AccentColor = ((SolidColorBrush)Global.ThemeResource["ApplicationAccentColor"]).Color;
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
                MainContentViewBackgroundAnimation.To = ((SolidColorBrush)Global.ThemeResource["MainContentViewBackground"]).Color;
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
        }

        private void ToBackClick(object sender, RoutedEventArgs e) {
            MainWindowRouter.Back();
        }

    }
}
