using CommonUtil.Route;
using CommonUtil.Store;
using CommonUtil.View;
using System;
using System.Windows;
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

        public MainWindow() {
            InitializeComponent();
            _ = new MainWindowRouter(ContentFrame);
            ContentFrame.Navigated += ContentFrameNavigated;
            Widget.MessageBox.PanelChildren = MessageBoxPanel.Children;  // 初始化
            MainWindowRouter.ToView(MainWindowRouter.RouterView.MainContent);
        }

        /// <summary>
        /// navigation 改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentFrameNavigated(object sender, NavigationEventArgs e) {
            Type contentType = e.Content.GetType();
            IsBackIconVisible = contentType != typeof(MainContentView);
            if (contentType == typeof(MainContentView)) {
                RouteViewTitle = Global.AppTitle;
            } else {
                foreach (var item in Global.MenuItems) {
                    if (item.ClassType == contentType) {
                        RouteViewTitle = item.Name;
                        break;
                    }
                }
            }
        }

        private void ToBackClick(object sender, RoutedEventArgs e) {
            MainWindowRouter.ToBack();
        }
    }
}
