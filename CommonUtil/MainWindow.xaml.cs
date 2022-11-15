﻿using CommonUITools.Route;
using CommonUITools.Utils;
using CommonUtil.Store;
using CommonUtil.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    private readonly RouterService RouterService;
    private static readonly Stack<Frame> FrameStack = new();
    private static readonly IDictionary<Type, RouterService> PageRouterServiceDict = new Dictionary<Type, RouterService>();

    public static void PushRouteStack(Frame frame) {
        FrameStack.Push(frame);
        frame.Navigated += FrameNavigatedHandler;
    }

    private static void FrameNavigatedHandler(object sender, NavigationEventArgs e) {
        if (e.Navigator is Frame frame) {
            frame.Navigated -= FrameNavigatedHandler;
        }
        RouteChanged?.Invoke(null, null!);
    }

    public static void PushRouteStack(RouterService service) => PushRouteStack(service.Frame);

    /// <summary>
    /// 回退
    /// </summary>
    private static void GoBack() {
        if (!CanGoBack) {
            return;
        }
        var frame = FrameStack.Pop();
        while (!frame.CanGoBack) {
            frame = FrameStack.Pop();
        }
        frame.GoBack();
        frame.Navigated += FrameNavigatedHandler;
    }

    /// <summary>
    /// 获取当前页面所属 RouterService
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns> 
    public static RouterService? GetCurrentRouteService(Type type)
        => PageRouterServiceDict.TryGetValue(type, out RouterService? routerService) ? routerService : null;
    /// <summary>
    /// 是否可以回退
    /// </summary>
    private static bool CanGoBack {
        get => FrameStack.Any(f => f.CanGoBack);
    }
    private readonly IEnumerable<Type> PageTypes;
    private static event EventHandler? RouteChanged;

    public MainWindow() {
        InitializeComponent();
        var types = Global.MenuItems.Select(t => t.ClassType).ToList();
        types.Add(typeof(MainContentView));
        PageTypes = types;
        RouterService = new(ContentFrame, PageTypes);
        AddPageFrame(PageTypes, RouterService);
        #region 设置 Storyboard
        TitleBarStoryboard = (Storyboard)Resources["TitleBarStoryboard"];
        MainContentViewLoadStoryboard = (Storyboard)Resources["MainContentViewLoadStoryboard"];
        TranslateTransformXAnimation = (DoubleAnimation)TitleBarStoryboard.Children.First(t => t.Name == "TranslateTransformX");
        MainContentViewBackgroundAnimation = (ColorAnimation)MainContentViewLoadStoryboard.Children.First(t => t.Name == "BackgroundAnimation");
        #endregion 
        CommonUITools.App.RegisterWidgetPage(this);
        RouteChanged += (_, _) => {
            IsBackIconVisible = CanGoBack;
        };
        // 延迟加载，减少卡顿
        Loaded += (_, _) => Task.Run(() => {
            Thread.Sleep(1000);
            Dispatcher.BeginInvoke(() => {
                RouterService.Navigate(typeof(MainContentView));
                PushRouteStack(ContentFrame);
            });
        });
    }

    /// <summary>
    /// 添加页面所属 RouterService
    /// </summary>
    /// <param name="pageTypes"></param>
    /// <param name="service"></param>
    private static void AddPageFrame(IEnumerable<Type> pageTypes, RouterService service)
        => pageTypes.ForEach(r => PageRouterServiceDict[r] = service);

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

    private void ToBackClick(object sender, RoutedEventArgs e) {
        GoBack();
    }

}

