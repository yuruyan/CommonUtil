using CommonUITools.Route;
using ModernWpf.Controls;
using NLog;
using System;
using System.Linq;
using System.Windows;

namespace CommonUtil.View;

public partial class TextToolView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly Type[] Routers = {
        typeof(RemoveDuplicateView),
        typeof(WhiteSpaceProcessView),
        typeof(EnglishTextProcessView),
        typeof(PrependLineNumberView),
        typeof(AddEnglishWordBraces),
    };
    private readonly RouterService RouterService;

    public TextToolView() {
        InitializeComponent();
        RouterService = new(ContentFrame, Routers);
    }

    /// <summary>
    /// 路由跳转
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void NavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
        if (args.SelectedItem is FrameworkElement element) {
            foreach (var item in Routers) {
                RouterService.Navigate(Routers.First(r => r.Name == element.Name));
            }
        }
    }
}

