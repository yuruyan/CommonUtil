namespace CommonUtil.View;

public partial class NavigationContentView : Page {
    internal static readonly DependencyPropertyKey ToolMenuItemsPropertyKey = DependencyProperty.RegisterReadOnly("ToolMenuItems", typeof(ExtendedObservableCollection<ToolMenuItemDO>), typeof(NavigationContentView), new PropertyMetadata());
    public static readonly DependencyProperty ToolMenuItemsProperty = ToolMenuItemsPropertyKey.DependencyProperty;

    public ExtendedObservableCollection<ToolMenuItemDO> ToolMenuItems => (ExtendedObservableCollection<ToolMenuItemDO>)GetValue(ToolMenuItemsProperty);

    public NavigationContentView() {
        SetValue(ToolMenuItemsPropertyKey, new ExtendedObservableCollection<ToolMenuItemDO>());
        InitializeComponent();
    }

    private void SelectedMenuChangedHandler(object sender, Type e) {
        Console.WriteLine(e.Name);
    }

    private void PageClosedHandler(object sender, Type e) {
        Console.WriteLine(e.Name);
        if (ToolMenuItems.Count == 0) {
            Console.WriteLine("empty list");
        }
    }
}

