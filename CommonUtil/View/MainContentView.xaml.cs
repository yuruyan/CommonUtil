using CommonUtil.Store;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CommonUtil.View;

public partial class MainContentView : Page {
    private static readonly DependencyPropertyKey MenuItemsKey = DependencyProperty.RegisterReadOnly("MenuItems", typeof(IList<ToolMenuItem>), typeof(MainContentView), new PropertyMetadata());
    public static readonly DependencyProperty MenuItemsProperty = MenuItemsKey.DependencyProperty;
    /// <summary>
    /// 菜单项目列表
    /// </summary>
    public IList<ToolMenuItem> ToolMenuItems {
        get => (IList<ToolMenuItem>)GetValue(MenuItemsProperty);
    }
    private SolidColorBrush MainContentViewBackground;
    private Window Window;

    public MainContentView() {
        SetValue(MenuItemsKey, Global.MenuItems.ToList());
        InitializeComponent();
    }

    private void RootLoaded(object sender, RoutedEventArgs e) {
        e.Handled = true;
        MainContentViewBackground ??= (SolidColorBrush)FindResource("MainContentViewBackground");
        Window ??= Window.GetWindow(this);
        Window.Background = MainContentViewBackground;
    }

    private void RootUnloaded(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Window.Background = new SolidColorBrush(Colors.White);
    }

    /// <summary>
    /// 点击菜单
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is ToolMenuItem menuItem) {
            CommonUITools.Route.RouterService? routerService = MainWindow.GetCurrentRouteService(this.GetType());
            if (routerService != null) {
                routerService.Navigate(menuItem.ClassType);
                MainWindow.PushRouteStack(routerService);
            }
        }
    }

}
