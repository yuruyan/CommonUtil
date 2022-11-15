using CommonUtil.Store;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        //Global.MenuItems.ForEach(item => {
        //    ToolMenuItems.Add(item);
        //});
        Task.Run(() => {
            Thread.Sleep(500);
            Global.MenuItems.ForEach(item => {
                Thread.Sleep(1);
                Dispatcher.Invoke(() => ToolMenuItems.Add(item));
            });
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
                routerService.Navigate(menuItem.ClassType);
                MainWindow.PushRouteStack(routerService);
            }
        }
    }

    private double BeginTime = 0;
    private double Interval = 40;
    private TimeSpan CurrentTimeSpan => TimeSpan.FromMilliseconds(BeginTime += (Interval--));
    private readonly IEasingFunction EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
    private readonly Duration Duration = new(TimeSpan.FromMilliseconds(500));

    /// <summary>
    /// 加载动画，只在初始化时执行
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is not FrameworkElement element) {
            return;
        }
        element.Loaded -= MenuItemLoadedHandler;
        var translateYAnimation = new DoubleAnimation(25, 0, Duration) {
            BeginTime = CurrentTimeSpan,
            EasingFunction = EasingFunction
        };
        var opacityAnimation = new DoubleAnimation(0.5, 1, Duration) {
            BeginTime = CurrentTimeSpan,
            EasingFunction = EasingFunction
        };
        Storyboard.SetTarget(translateYAnimation, element);
        Storyboard.SetTarget(opacityAnimation, element);
        Storyboard.SetTargetProperty(translateYAnimation, new("RenderTransform.(TranslateTransform.Y)"));
        Storyboard.SetTargetProperty(opacityAnimation, new("Opacity"));
        new Storyboard() {
            Children = { translateYAnimation, opacityAnimation }
        }.Begin();
    }
}
