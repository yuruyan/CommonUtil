using CommonUtil.Route;
using CommonUtil.View;
using System;
using System.Collections.Generic;

namespace CommonUtil.Store {
    public class Global {
        public static readonly string AppTitle = "工具集";
        public static readonly string ImagePath = "/Resource/image/";

        public static readonly List<ToolMenuItem> MenuItems = new() {
            new() { Name = "Base64 编码/解码", RouteView = MainWindowRouter.RouterView.Base64Tool, ImagePath = ImagePath + "base64.svg", ClassType = typeof(Base64ToolView) },
        };
    }

    public class ToolMenuItem {
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public MainWindowRouter.RouterView RouteView { get; set; } = MainWindowRouter.RouterView.MainContent;
        public Type ClassType { get; set; } = typeof(MainContentView);
    }
}
