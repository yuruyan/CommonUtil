using ModernWpf.Controls;
using WindowsInput.Events;

namespace CommonUtil.View;

public partial class MouseScrollDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty ScrollOffsetProperty = DependencyProperty.Register("ScrollOffset", typeof(double), typeof(MouseScrollDialog), new PropertyMetadata(100.0));
    private static readonly IReadOnlyDictionary<string, ButtonCode> ButtonCodes = new Dictionary<string, ButtonCode>() {
        {"水平滚动", ButtonCode.HScroll},
        {"垂直滚动", ButtonCode.VScroll},
    };
    private readonly IList<string> ButtonCodeKeys;

    /// <summary>
    /// 滚动偏移量
    /// </summary>
    public double ScrollOffset {
        get { return (double)GetValue(ScrollOffsetProperty); }
        set { SetValue(ScrollOffsetProperty, value); }
    }

    public MouseScrollDialog() {
        ButtonCodeKeys = ButtonCodes.Keys.ToList();
        AutomationMethod = DesktopAutomation.MouseScroll;
        DescriptionHeader = "滚动鼠标";
        InitializeComponent();
        ScrollDirectionComboBox.ItemsSource = ButtonCodeKeys;
        ScrollDirectionComboBox.SelectedIndex = 0;
    }

    /// <summary>
    /// Set values
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="e"></param>
    private void ClosingHandler(ContentDialog dialog, ContentDialogClosingEventArgs e) {
        var value = ButtonCodeKeys[ScrollDirectionComboBox.SelectedIndex];
        Parameters = new object[] {
            ButtonCodes[value],
            (int)ScrollOffset,
        };
        DescriptionValue = $"{value}, {ScrollOffset}";
    }
}
