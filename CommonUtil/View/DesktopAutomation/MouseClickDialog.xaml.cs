using ModernWpf.Controls;
using WindowsInput.Events;

namespace CommonUtil.View;

public partial class MouseClickDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly IReadOnlyDictionary<string, ButtonCode> ButtonCodes = new Dictionary<string, ButtonCode>() {
        {"左键", ButtonCode.Left},
        {"右键", ButtonCode.Right},
        {"中键", ButtonCode.Middle},
        {"水平滚动", ButtonCode.HScroll},
        {"垂直滚动", ButtonCode.VScroll},
    };
    private readonly IList<string> ButtonCodeKeys;

    public MouseClickDialog() {
        ButtonCodeKeys = ButtonCodes.Keys.ToList();
        AutomationMethod = DesktopAutomation.MouseClick;
        DescriptionHeader = "点击鼠标";
        InitializeComponent();
        ButtonCodeComboBox.ItemsSource = ButtonCodeKeys;
        ButtonCodeComboBox.SelectedIndex = 0;
    }

    /// <summary>
    /// Set values
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="e"></param>
    private void ClosingHandler(ContentDialog dialog, ContentDialogClosingEventArgs e) {
        var value = ButtonCodeKeys[ButtonCodeComboBox.SelectedIndex];
        Parameters = new object[] {
            ButtonCodes[value]
        };
        DescriptionValue = value;
    }
}
