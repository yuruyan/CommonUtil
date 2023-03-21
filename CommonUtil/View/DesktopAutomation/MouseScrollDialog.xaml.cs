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
    private static readonly IList<string> ButtonCodeKeys = ButtonCodes.Keys.ToList();
    private static readonly IList<ButtonCode> ButtonCodeValues = ButtonCodes.Values.ToList();

    /// <summary>
    /// 滚动偏移量
    /// </summary>
    public double ScrollOffset {
        get { return (double)GetValue(ScrollOffsetProperty); }
        set { SetValue(ScrollOffsetProperty, value); }
    }

    public MouseScrollDialog() {
        AutomationMethod = DesktopAutomation.MouseScroll;
        Title = DescriptionHeader = "滚动鼠标";
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
        _ = dialog;
        _ = e;
        var value = ButtonCodeKeys[ScrollDirectionComboBox.SelectedIndex];
        Parameters = new object[] {
            ButtonCodes[value],
            (int)ScrollOffset,
        };
        DescriptionValue = $"{value}, {ScrollOffset}";
    }

    public override void ParseParameters(object[] parameters) {
        ScrollDirectionComboBox.SelectedIndex = ButtonCodeValues.IndexOf((ButtonCode)parameters[0]);
        // Unboxing failure: double type cast
        ScrollOffset = (int)parameters[1];
    }
}
