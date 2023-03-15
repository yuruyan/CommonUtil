namespace CommonUtil.View.Navigation;

public partial class NavigationContentListView : UserControl {
    public static readonly DependencyProperty ToolMenuItemsProperty = DependencyProperty.Register("ToolMenuItems", typeof(ExtendedObservableCollection<ToolMenuItemDO>), typeof(NavigationContentListView), new PropertyMetadata());
    /// <summary>
    /// 选中项改变，参数为 ViewType
    /// </summary>
    public event EventHandler<Type>? SelectedMenuChanged;
    /// <summary>
    /// 关闭页面，参数为 ViewType
    /// </summary>
    public event EventHandler<Type>? Closed;

    public ExtendedObservableCollection<ToolMenuItemDO> ToolMenuItems {
        get { return (ExtendedObservableCollection<ToolMenuItemDO>)GetValue(ToolMenuItemsProperty); }
        set { SetValue(ToolMenuItemsProperty, value); }
    }

    public NavigationContentListView() {
        InitializeComponent();
    }

    /// <summary>
    /// 关闭页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClosePageClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var menuItem = sender.GetElementDataContext<ToolMenuItemDO>();
        if (menuItem is not null) {
            ToolMenuItems.Remove(menuItem);
            Closed?.Invoke(sender, menuItem.ViewType);
        }
    }

    /// <summary>
    /// 选中项变化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemSelectionChangedHandler(object sender, SelectionChangedEventArgs e) {
        e.Handled = true;
        var item = e.AddedItems.OfType<ToolMenuItemDO>().FirstOrDefault();
        if (item is not null) {
            SelectedMenuChanged?.Invoke(sender, item.ViewType);
        }
    }
}
