using CommonUtil.Store;
using CommonUtil.View;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;

namespace CommonUtil.Route;

public class MainWindowRouter {
    private static RouterService RouterService;

    public MainWindowRouter(Frame frame) {
        var routers = new List<Type>();
        routers.Add(typeof(MainContentView));
        foreach (var item in Global.MenuItems) {
            routers.Add(item.ClassType);
        }
        RouterService = new RouterService(frame, routers);
    }

    public static void Navigate(Type viewType) {
        RouterService.Navigate(viewType);
    }

    public static void Back() {
        RouterService.Back();
    }

}
