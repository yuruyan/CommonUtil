using CommonUtil.Store;
using CommonUtil.View;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using System;
using System.Collections.Generic;

namespace CommonUtil.Route {
    public class MainWindowRouter {
        private static Frame Frame;
        private static NavigationTransitionInfo NavigationTransitionInfo = new DrillInNavigationTransitionInfo();
        private static readonly Dictionary<RouterView, Type> NavigationMap = new();

        public MainWindowRouter(Frame frame) {
            Frame = frame;
            NavigationMap.Add(RouterView.MainContent, typeof(MainContentView));
            foreach (var item in Global.MenuItems) {
                NavigationMap.Add(item.RouteView, item.ClassType);
            }
        }

        public static void ToView(RouterView view, object? args = null) {
            Frame?.Navigate(NavigationMap[view], args, NavigationTransitionInfo);
        }

        public static void ToBack() {
            if (Frame == null) {
                return;
            }
            if (Frame.CanGoBack) {
                Frame.GoBack(NavigationTransitionInfo);
            }
        }

        public enum RouterView {
            MainContent,
            Base64Tool,
        }
    }
}
