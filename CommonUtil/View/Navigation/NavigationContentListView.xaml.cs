using System.Collections.Specialized;

namespace CommonUtil.View.Navigation;

public partial class NavigationContentListView : UserControl {
    public static readonly DependencyProperty ToolMenuItemsProperty = DependencyProperty.Register("ToolMenuItems", typeof(ExtendedObservableCollection<ToolMenuItemDO>), typeof(NavigationContentListView), new PropertyMetadata(ToolMenuItemsPropertyChangedHandler));

    public ExtendedObservableCollection<ToolMenuItemDO> ToolMenuItems {
        get { return (ExtendedObservableCollection<ToolMenuItemDO>)GetValue(ToolMenuItemsProperty); }
        set { SetValue(ToolMenuItemsProperty, value); }
    }

    /// <summary>
    /// 选中项改变，参数为 ViewType, 当为 null 时，则表示 <see cref="ToolMenuItems"/> 为空
    /// </summary>
    public event EventHandler<Type?>? SelectedMenuChanged;
    /// <summary>
    /// 关闭页面，参数为 ViewType
    /// </summary>
    public event EventHandler<Type>? Closed;
    private const string RootLoadingStoryboardName = "RootLoadingStoryboard";
    private readonly Storyboard RootLoadingStoryboard;
    private Type? CurrentPageType;

    public NavigationContentListView() {
        InitializeComponent();
        RootLoadingStoryboard = (Storyboard)Resources[RootLoadingStoryboardName];
        // Begin Animation
        IsVisibleChanged += (s, e) => {
            if (e.NewValue is true) {
                RootLoadingStoryboard.Begin();
            }
        };
    }

    /// <summary>
    /// 数据改变，选中第一个
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void ToolMenuItemsPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not NavigationContentListView self) {
            return;
        }
        self.ToolMenuItems.CollectionChanged -= self.ToolMenuItemsCollectionChangedHandler;
        self.ToolMenuItems.CollectionChanged += self.ToolMenuItemsCollectionChangedHandler;
        // Select first item
        if (self.ToolMenuItems.FirstOrDefault() is ToolMenuItemDO firstItem) {
            self.SelectItem(firstItem.ViewType);
        }
    }

    private void ToolMenuItemsCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Reset) {
            // When the first item was added
            if (CurrentPageType == null && e.NewItems?.Count > 0) {
                SelectItem(ToolMenuItems.First().ViewType);
            }
        }
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
            // Empty list
            if (ToolMenuItems.Count == 0) {
                CurrentPageType = null;
                SelectedMenuChanged?.Invoke(sender, null);
            }
            // Close current page, navigate to first item
            else if (CurrentPageType == menuItem.ViewType) {
                SelectItem(ToolMenuItems.First().ViewType);
            }
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
            CurrentPageType = item.ViewType;
            SelectedMenuChanged?.Invoke(sender, item.ViewType);
        }
    }

    /// <summary>
    /// 选中菜单项
    /// </summary>
    /// <param name="viewType"></param>
    public void SelectItem(Type viewType) {
        var targetIndex = ToolMenuItems.IndexOf(item => item.ViewType == viewType);
        if (targetIndex != -1) {
            MenuItemListBox.SelectedIndex = targetIndex;
        }
    }

    private void MenuItemListBoxVisibleChangedHandler(object sender, DependencyPropertyChangedEventArgs e) {
        if (sender is FrameworkElement element) {
            if (e.NewValue is true) {
                RevealBackgroundHelper.SetIsEnabled(element, true);
                element.SetResourceReference(RevealBackgroundHelper.BrushColorProperty, "ApplicationForeground");
                RevealBackgroundHelper.SetBackgroundProperty(element, BackgroundProperty);
                return;
            }
            RevealBackgroundHelper.Dispose(element);
        }
    }

    private void MenuItemListBoxLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is FrameworkElement element) {
            RevealBackgroundHelper.SetIsEnabled(element, true);
            element.SetResourceReference(RevealBackgroundHelper.BrushColorProperty, "ApplicationForeground");
            RevealBackgroundHelper.SetBackgroundProperty(element, BackgroundProperty);
        }
    }

    private void MenuItemListBoxUnloadedHandler(object sender, RoutedEventArgs e) {
        if (sender is FrameworkElement element) {
            RevealBackgroundHelper.Dispose(element);
        }
    }
}
