using CommonUITools.Route;
using CommonUtil.Store;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CommonUtil.View;

public partial class MainContentView : Page {
    private static readonly DependencyPropertyKey MenuItemsKey = DependencyProperty.RegisterReadOnly("MenuItems", typeof(ObservableCollection<ToolMenuItem>), typeof(MainContentView), new PropertyMetadata());
    public static readonly DependencyProperty MenuItemsProperty = MenuItemsKey.DependencyProperty;

    /// <summary>
    /// 菜单项目列表
    /// </summary>
    public ObservableCollection<ToolMenuItem> ToolMenuItems {
        get => (ObservableCollection<ToolMenuItem>)GetValue(MenuItemsProperty);
    }
    private SolidColorBrush MainContentViewBackground = default!;
    private Window Window = default!;

    public MainContentView() {
        SetValue(MenuItemsKey, new ObservableCollection<ToolMenuItem>());
        Loaded += InitializeLoadedHandler;
        InitializeComponent();
    }

    /// <summary>
    /// 执行一次
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InitializeLoadedHandler(object sender, RoutedEventArgs e) {
        Loaded -= InitializeLoadedHandler;
        MainContentViewBackground = (SolidColorBrush)FindResource("MainContentViewBackground");
        Window = Window.GetWindow(this);
        // 延迟加载
        Task.Run(() => {
            Thread.Sleep(500);
            Dispatcher.Invoke(() => Global.MenuItems.ForEach(ToolMenuItems.Add));
        });
    }

    private void RootLoaded(object sender, RoutedEventArgs e) {
        e.Handled = true;
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
            var routerService = MainWindow.GetCurrentRouteService(this.GetType());
            if (routerService != null) {
                routerService.Navigate(menuItem.ClassType, NavigationTransitionEffect.DrillIn);
                MainWindow.PushRouteStack(routerService);
            }
        }
    }
}
