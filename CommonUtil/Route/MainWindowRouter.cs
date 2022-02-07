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
        private static readonly Dictionary<RouterView, RouterInfo> NavigationMap = new();

        public MainWindowRouter(Frame frame) {
            Frame = frame;
            NavigationMap.Add(RouterView.MainContent, new() { ClassType = typeof(MainContentView) });
            foreach (var item in Global.MenuItems) {
                NavigationMap.Add(item.RouteView, new() { ClassType = item.ClassType });
            }
        }

        public static void ToView(RouterView view, object? args = null) {
            NavigationMap[view].Instance = NavigationMap[view].Instance ?? Activator.CreateInstance(NavigationMap[view].ClassType);
            Frame?.Navigate(NavigationMap[view].Instance, args);
        }

        public static void ToBack() {
            if (Frame == null) {
                return;
            }
            if (Frame.CanGoBack) {
                Frame.GoBack(NavigationTransitionInfo);
            }
        }

        private class RouterInfo {
            public Type ClassType { get; set; } = typeof(object);
            public object Instance { get; set; }
        }

        public enum RouterView {
            MainContent,
            Base64Tool,
            RandomGenerator,
            ChineseTransform,
            TimeStamp,
            FileMergeSplit,
            DataDigest,
            AsciiTable,
            BMICalculator,
        }
    }
}
