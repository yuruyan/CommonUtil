using CommonUITools.Route;
using ModernWpf.Controls;
using NLog;
using System;
using System.Windows;

namespace CommonUtil.View;

public partial class CodeGeneratorView : System.Windows.Controls.Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly Type[] Routers = {
        typeof(CSharpDependencyView),
    };
    private readonly RouterService _RouterService;

    public CodeGeneratorView() {
        InitializeComponent();
        _RouterService = new(ContentFrame, Routers);
    }

    /// <summary>
    /// 路由跳转
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void NavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
        if (args.SelectedItem is FrameworkElement element) {
            foreach (var item in Routers) {
                if (item.Name.Contains(element.Name)) {
                    _RouterService.Navigate(item);
                    break;
                }
            }
        }
    }
}

