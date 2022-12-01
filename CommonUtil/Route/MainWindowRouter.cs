using System.Windows.Navigation;

namespace CommonUITools.Route;

internal static class MainWindowRouter {
    private static readonly Stack<Frame> FrameStack = new();
    private static readonly IDictionary<Type, RouterService> PageRouterServiceDict = new Dictionary<Type, RouterService>();
    public static event EventHandler? RouteChanged;
    /// <summary>
    /// 是否可以回退
    /// </summary>
    public static bool CanGoBack {
        get => FrameStack.Any(f => f.CanGoBack);
    }

    /// <summary>
    /// 回退
    /// </summary>
    public static void GoBack() {
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

    public static void PushRouteStack(Frame frame) {
        FrameStack.Push(frame);
        frame.Navigated += FrameNavigatedHandler;
    }

    public static void PushRouteStack(RouterService service) => PushRouteStack(service.Frame);

    private static void FrameNavigatedHandler(object sender, NavigationEventArgs e) {
        if (e.Navigator is Frame frame) {
            frame.Navigated -= FrameNavigatedHandler;
        }
        RouteChanged?.Invoke(null, null!);
    }

    /// <summary>
    /// 添加页面所属 RouterService
    /// </summary>
    /// <param name="pageTypes"></param>
    /// <param name="service"></param>
    public static void AddPageFrame(IEnumerable<Type> pageTypes, RouterService service)
        => pageTypes.ForEach(r => PageRouterServiceDict[r] = service);

}
