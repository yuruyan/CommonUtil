namespace CommonUtil.View;

public partial class NavigationContentView : Page, INavigationRequest<NavigationRequestArgs>, INavigationService {
    internal static readonly DependencyPropertyKey ToolMenuItemsPropertyKey = DependencyProperty.RegisterReadOnly("ToolMenuItems", typeof(ExtendedObservableCollection<ToolMenuItemDO>), typeof(NavigationContentView), new PropertyMetadata());
    public static readonly DependencyProperty ToolMenuItemsProperty = ToolMenuItemsPropertyKey.DependencyProperty;
    private readonly RouterService RouterService;
    public event EventHandler<NavigationRequestArgs>? NavigationRequested;
    ///// <summary>
    ///// 每一个内容页面关闭事件
    ///// </summary>
    //public event EventHandler<ToolMenuItemDO>? ContentViewClosed;
    public ExtendedObservableCollection<ToolMenuItemDO> ToolMenuItems => (ExtendedObservableCollection<ToolMenuItemDO>)GetValue(ToolMenuItemsProperty);

    public NavigationContentView() {
        SetValue(ToolMenuItemsPropertyKey, new ExtendedObservableCollection<ToolMenuItemDO>());
        InitializeComponent();
        RouterService = GetRouterService();
        AutoHideHelper.EnableAutoHideOnClickOuter(NavigationContentListView, ElementVisibilityHelperNames.NavigationContentListView);
    }

    private RouterService GetRouterService() {
        return new RouterService(
            ContentFrame,
            Global.MenuItems.Select(m => m.ClassType)
        );
    }

    /// <summary>
    /// 菜单选中项改变
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="viewType"></param>
    private void SelectedMenuChangedHandler(object _, Type viewType) {
        RouterService.Navigate(viewType);
    }

    /// <summary>
    /// 页面关闭
    /// </summary>
    /// <param name="_"></param>
    /// <param name="viewType"></param>
    private void PageClosedHandler(object _, Type viewType) {
        #region Clean Memory
        if (RouterService.GetInstance(viewType) is IDisposable disposable) {
            disposable.Dispose();
        }
        RouterService.RemovePage(viewType);
        #endregion

        // Navigate to MainContentView
        if (ToolMenuItems.Count == 0) {
            NavigationRequested?.Invoke(
                this,
                new(typeof(MainContentView))
            );
        }
    }

    /// <summary>
    /// 导航到目标页面
    /// </summary>
    /// <param name="data">The type of target page</param>
    public void Navigated(object? data) {
        if (data is Type pageType) {
            // 不已存在实例
            if (ToolMenuItems.IndexOf(item => item.ViewType == pageType) == -1) {
                ToolMenuItems.Add(MapperUtils.Instance.Map<ToolMenuItemDO>(
                    DataSet.ToolMenuItems.First(src => src.ClassType == pageType)
                ));
            }
            // Select the page
            NavigationContentListView.SelectItem(pageType);
        }
    }

    /// <summary>
    /// Click to close NavigationContentListView
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseNavigationContentListViewHandler(object sender, MouseButtonEventArgs e) {
        // Can't set 'e.Handled = true'
        if (sender is UIElement element) {
            element.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Hide ListView
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewLoadedHandler(object sender, RoutedEventArgs e) {
        NavigationContentListView.Visibility = Visibility.Collapsed;
    }
}

