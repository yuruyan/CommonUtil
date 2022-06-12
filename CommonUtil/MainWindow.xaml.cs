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
        // 标题动画
        private Storyboard? TitleBarStoryboard;
        private DoubleAnimation? TranslateTransformXAnimation;

        public MainWindow() {
            InitializeComponent();
            if (Resources["TitleBarStoryboard"] is Storyboard storyboard) {
                TitleBarStoryboard = storyboard;
                Timeline? timeline = storyboard.Children.FirstOrDefault(t => t.Name == "TranslateTransformX");
                if (timeline is DoubleAnimation animation) {
                    TranslateTransformXAnimation = animation;
                }
            }
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
                if (TranslateTransformXAnimation != null) {
                    TranslateTransformXAnimation.From = -100;
                }
            } else {
                foreach (var item in Global.MenuItems) {
                    if (item.ClassType == contentType) {
                        RouteViewTitle = item.Name;
                        break;
                    }
                }
                if (TranslateTransformXAnimation != null) {
                    TranslateTransformXAnimation.From = 100;
                }
            }
            TitleBarStoryboard?.Begin();
        }

        private void ToBackClick(object sender, RoutedEventArgs e) {
            MainWindowRouter.Back();
        }

    }
}
