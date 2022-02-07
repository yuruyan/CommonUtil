using CommonUtil.Route;
using CommonUtil.Store;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommonUtil.View {
    public partial class MainContentView : Page {
        /// <summary>
        /// 菜单项目列表
        /// </summary>
        public List<ToolMenuItem> ToolMenuItems {
            get { return (List<ToolMenuItem>)GetValue(MenuItemsProperty); }
            set { SetValue(MenuItemsProperty, value); }
        }
        public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.Register("MenuItems", typeof(List<ToolMenuItem>), typeof(MainContentView), new PropertyMetadata());

        public MainContentView() {
            ToolMenuItems = new();
            foreach (var item in Global.MenuItems) {
                ToolMenuItems.Add(item);
            }
            InitializeComponent();
        }

        private void RootLoaded(object sender, RoutedEventArgs e) {

        }

        /// <summary>
        /// 点击菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if(sender is FrameworkElement element) {
                if(element.DataContext is ToolMenuItem menuItem) {
                    //RouterView = menuItem.RouteView;
                    //RouterViewTitle = menuItem.Name;
                    MainWindowRouter.Navigate(menuItem.ClassType);
                }
            }
        }

    }
}
