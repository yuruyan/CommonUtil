namespace CommonUtil.View;

public partial class DesktopAutomationView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty AutomationItemsProperty = DependencyProperty.Register("AutomationItems", typeof(IList<AutomationItem>), typeof(DesktopAutomationView), new PropertyMetadata());

    /// <summary>
    /// 菜单
    /// </summary>
    public IList<AutomationItem> AutomationItems {
        get { return (IList<AutomationItem>)GetValue(AutomationItemsProperty); }
        set { SetValue(AutomationItemsProperty, value); }
    }

    public DesktopAutomationView() {
        // Place before InitializeComponent();
        InitAutomationItems();
        InitializeComponent();
    }

    /// <summary>
    /// 初始化 AutomationItems
    /// </summary>
    private void InitAutomationItems() {
        AutomationItems = new List<AutomationItem> {
            new AutomationItem("键盘", "\ue629", true) {
                Children = new List<AutomationItem> {
                    new AutomationItem("键入文本", "\ue627"),
                    new AutomationItem("按下键盘", "\ue62a"),
                    new AutomationItem("输入快捷键", "\ue62a"),
                }
            },
            new AutomationItem("鼠标", "\ue651", true) {
                Children = new List<AutomationItem> {
                    new AutomationItem("单击鼠标", "\ue645"),
                    new AutomationItem("双击鼠标", "\ue60e"),
                    new AutomationItem("移动鼠标", "\ue648"),
                    new AutomationItem("滚动鼠标", "\ue628"),
                }
            },
        };
    }
}
