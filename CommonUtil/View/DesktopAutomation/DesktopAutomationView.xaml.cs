namespace CommonUtil.View;

public partial class DesktopAutomationView : Page {
    private class AutomationDialogItem {
        public Type DialogType { get; }
        public DesktopAutomationDialog? Dialog { get; set; }

        public AutomationDialogItem(Type dialogType) {
            DialogType = dialogType;
        }
    }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty AutomationItemsProperty = DependencyProperty.Register("AutomationItems", typeof(IList<AutomationItem>), typeof(DesktopAutomationView), new PropertyMetadata());
    public static readonly DependencyProperty AutomationStepsProperty = DependencyProperty.Register("AutomationSteps", typeof(ExtendedObservableCollection<AutomationStep>), typeof(DesktopAutomationView), new PropertyMetadata());

    /// <summary>
    /// 执行步骤
    /// </summary>
    public ExtendedObservableCollection<AutomationStep> AutomationSteps {
        get { return (ExtendedObservableCollection<AutomationStep>)GetValue(AutomationStepsProperty); }
        set { SetValue(AutomationStepsProperty, value); }
    }
    /// <summary>
    /// 菜单
    /// </summary>
    public IList<AutomationItem> AutomationItems {
        get { return (IList<AutomationItem>)GetValue(AutomationItemsProperty); }
        private set { SetValue(AutomationItemsProperty, value); }
    }
    private static readonly IReadOnlyDictionary<uint, AutomationDialogItem> AutomationItemDialogDict = new Dictionary<uint, AutomationDialogItem>() {
        {1, new (typeof(InputTextDialog)) },
        {2, new (typeof(PressKeyDialog)) },
        {3, new (typeof(PressKeyShortcutDialog)) },
        {4, new (typeof(MouseClickDialog)) },
        {5, new (typeof(MouseDoubleClickDialog)) },
        {6, new (typeof(MouseMoveDialog)) },
        {7, new (typeof(MouseScrollDialog)) },
    };

    public DesktopAutomationView() {
        AutomationSteps = new();
        // Place before InitializeComponent();
        InitAutomationItems();
        InitializeComponent();
    }

    /// <summary>
    /// 初始化 AutomationItems
    /// </summary>
    private void InitAutomationItems() {
        AutomationItems = new List<AutomationItem> {
            new AutomationItem("键盘", "\ue629",isFolder: true) {
                Children = new List<AutomationItem> {
                    new AutomationItem("键入文本", "\ue627", id: 1),
                    new AutomationItem("按下键盘", "\ue62a", id: 2),
                    new AutomationItem("输入快捷键", "\ue62a", id: 3),
                }
            },
            new AutomationItem("鼠标", "\ue651",isFolder: true) {
                Children = new List<AutomationItem> {
                    new AutomationItem("单击鼠标", "\ue645", id:4),
                    new AutomationItem("双击鼠标", "\ue60e", id:5),
                    new AutomationItem("移动鼠标", "\ue648", id:6),
                    new AutomationItem("滚动鼠标", "\ue628", id:7),
                }
            },
        };
    }

    /// <summary>
    /// 双击鼠标
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MenuMouseDoubleClickHandler(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        if (e.ChangedButton != MouseButton.Left) {
            return;
        }
        if (e.OriginalSource is not FrameworkElement element
            || element.DataContext is not AutomationItem automationItem
            || automationItem.IsFolder
        ) {
            return;
        }
        var dialogItem = AutomationItemDialogDict[automationItem.Id];
        // Initialize
        dialogItem.Dialog ??= (DesktopAutomationDialog)Activator.CreateInstance(AutomationItemDialogDict[automationItem.Id].DialogType)!;
        var dialog = dialogItem.Dialog;
        dialog.Title = automationItem.Name;
        // 确定
        if (await dialog.ShowAsync() == ModernWpf.Controls.ContentDialogResult.Primary) {
            AutomationSteps.Add(new(dialog.AutomationMethod, dialog.Parameters) {
                Icon = automationItem.Icon,
                DescriptionHeader = dialog.DescriptionHeader,
                DescriptionValue = dialog.DescriptionValue,
            });
        }
    }

    /// <summary>
    /// 双击步骤
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AutomationStepsListBoxMouseDoubleClickHandler(object sender, MouseButtonEventArgs e) {

    }
}
