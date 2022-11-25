using CommonUITools.Route;
using CommonUITools.Utils;
using CommonUtil.Store;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
    private Window Window = default!;

    public MainContentView() {
        SetValue(MenuItemsKey, new ObservableCollection<ToolMenuItem>());
        UIUtils.SetLoadedOnceEventHandler(this, InitializeLoadedHandler);
        InitializeComponent();
        // 监听主题变化
        ThemeManager.Current.ThemeChanged += (_, _) => {
            if (IsVisible) {
                var brush = (SolidColorBrush)FindResource("MainContentViewBackgroundBrush");
                SetWindowTitleBarBackground(brush);
                Background = brush;
            }
        };
    }

    /// <summary>
    /// 设置 MainWindow TitleBarBackground
    /// </summary>
    /// <param name="brush"></param>
    private void SetWindowTitleBarBackground(Brush brush) {
        if (Window is MainWindow window) {
            window.TitleBarBackground = brush;
        }
    }

    /// <summary>
    /// 执行一次
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InitializeLoadedHandler(object sender, RoutedEventArgs e) {
        Window = Window.GetWindow(this);
        // 延迟加载
        Task.Run(() => {
            Thread.Sleep(500);
            Dispatcher.Invoke(() => {
                Global.MenuItems.ForEach(ToolMenuItems.Add);
                if (Resources["InitializeStoryboard"] is Storyboard storyboard) {
                    storyboard.Completed += (_, _) => Opacity = 1;
                    storyboard.Begin();
                }
            });
        });
    }

    private void RootLoaded(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var brush = (SolidColorBrush)FindResource("MainContentViewBackgroundBrush");
        SetWindowTitleBarBackground(brush);
        Background = brush;
    }

    private void RootUnloaded(object sender, RoutedEventArgs e) {
        e.Handled = true;
        SetWindowTitleBarBackground(new SolidColorBrush(Colors.Transparent));
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
