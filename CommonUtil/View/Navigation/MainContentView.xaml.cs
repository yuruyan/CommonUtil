
namespace CommonUtil.View;

public partial class MainContentView : Page, INavigationRequest<NavigationRequestArgs> {
    private static readonly DependencyPropertyKey MenuItemsKey = DependencyProperty.RegisterReadOnly("MenuItems", typeof(ObservableCollection<ToolMenuItem>), typeof(MainContentView), new PropertyMetadata());
    public static readonly DependencyProperty MenuItemsProperty = MenuItemsKey.DependencyProperty;
    public static readonly DependencyProperty PreviousBackgroundColorProperty = DependencyProperty.Register("PreviousBackgroundColor", typeof(Color), typeof(MainContentView), new PropertyMetadata(Colors.Transparent));
    public static readonly DependencyProperty CurrentBackgroundColorProperty = DependencyProperty.Register("CurrentBackgroundColor", typeof(Color), typeof(MainContentView), new PropertyMetadata(Colors.Transparent));
    private const string ItemBackgroundBrushProxyKey = "MainContentViewItemBackgroundBrushProxy";
    private const string ItemBackgroundBrushKey = "MainContentViewItemBackgroundBrush";
    private const string MainContentViewBackgroundBrushKey = "MainContentViewBackgroundBrush";
    private readonly Storyboard MainContentViewBackgroundStoryboard;
    public event EventHandler<NavigationRequestArgs>? NavigationRequested;

    /// <summary>
    /// 菜单项目列表
    /// </summary>
    public ObservableCollection<ToolMenuItem> ToolMenuItems => (ObservableCollection<ToolMenuItem>)GetValue(MenuItemsProperty);
    /// <summary>
    /// From BackgroundColor
    /// </summary>
    public Color PreviousBackgroundColor {
        get { return (Color)GetValue(PreviousBackgroundColorProperty); }
        set { SetValue(PreviousBackgroundColorProperty, value); }
    }
    /// <summary>
    /// To BackgroundColor
    /// </summary>
    public Color CurrentBackgroundColor {
        get { return (Color)GetValue(CurrentBackgroundColorProperty); }
        set { SetValue(CurrentBackgroundColorProperty, value); }
    }

    public MainContentView() {
        SetValue(MenuItemsKey, new ObservableCollection<ToolMenuItem>());
        this.SetLoadedOnceEventHandler(InitializeLoadedHandler);
        InitializeComponent();
        #region 设置 Storyboard
        MainContentViewBackgroundStoryboard = (Storyboard)Resources["MainContentViewBackgroundStoryboard"];
        MainContentViewBackgroundStoryboard.Completed += (_, _) => {
            // 恢复
            Resources[ItemBackgroundBrushProxyKey] = FindResource(ItemBackgroundBrushKey);
        };
        #endregion
        WatchThemeChanged();
    }

    /// <summary>
    /// 监听主题变化
    /// </summary>
    private void WatchThemeChanged() {
        ThemeManager.Current.ThemeChanged += (_, _) => {
            if (IsVisible) {
                // 设置透明
                Resources[ItemBackgroundBrushProxyKey] = new SolidColorBrush();
                var brush = (SolidColorBrush)FindResource(MainContentViewBackgroundBrushKey);
                PreviousBackgroundColor = ((SolidColorBrush)Background).Color;
                CurrentBackgroundColor = brush.Color;
                MainContentViewBackgroundStoryboard.Begin();
            }
        };
    }

    /// <summary>
    /// 执行一次
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InitializeLoadedHandler(object sender, RoutedEventArgs e) {
        Dispatcher.InvokeAsync(() => {
            Global.MenuItems.ForEach(ToolMenuItems.Add);
            if (Resources["InitializeStoryboard"] is Storyboard storyboard) {
                storyboard.Completed += (_, _) => Opacity = 1;
                storyboard.Begin();
            }
        });
    }

    private void RootLoaded(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var brush = (SolidColorBrush)FindResource(MainContentViewBackgroundBrushKey);
        Resources[ItemBackgroundBrushProxyKey] = FindResource(ItemBackgroundBrushKey);
        Background = brush;
    }

    private void RootUnloaded(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Resources[ItemBackgroundBrushProxyKey] = FindResource(ItemBackgroundBrushKey);
    }

    /// <summary>
    /// 点击菜单
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        var menuItem = sender.GetElementDataContext<ToolMenuItem>();
        if (menuItem != default) {
            NavigationRequested?.Invoke(
                sender,
                new(typeof(NavigationContentView), menuItem.ClassType)
            );
        }
    }
}
