using CommonUITools.Route;
using ModernWpf.Controls;
using System.Linq;
using System.Windows;

namespace CommonUtil.Route;

internal static class NavigationUtils {
    /// <summary>
    /// 启用导航
    /// </summary>
    /// <param name="navigationView"></param>
    /// <param name="routerService"></param>
    /// <param name="frame"></param>
    /// <remarks>
    /// 需要在 xaml 里设置 NavigationItem，并且 Name 属性设置为 PageType.Name 属性
    /// </remarks>
    public static void EnableNavigation(
        NavigationView navigationView,
        RouterService routerService,
        Frame frame
    ) {
        // 主动导航跳转
        navigationView.SelectionChanged += (s, e) => {
            if (e.SelectedItem is FrameworkElement element) {
                routerService.Navigate(routerService.GetRouteTypes().First(t => t.Name == element.Name));
                MainWindow.PushRouteStack(routerService);
            }
        };
        // 自动导航跳转
        frame.Navigated += (s, e) => {
            navigationView.SelectedItem = navigationView.MenuItems
                .Cast<FrameworkElement>()
                .FirstOrDefault(item => item.Name == e.Content.GetType().Name);
        };
    }

}
