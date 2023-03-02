using ModernWpf.Controls;
using WindowsInput.Events;

namespace CommonUtil.View;

public partial class PressKeyDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IList<string> KeyCodeKeys;

    public PressKeyDialog() {
        KeyCodeKeys = Enum.GetNames<KeyCode>();
        AutomationMethod = DesktopAutomation.PressKey;
        DescriptionHeader = "按下键盘";
        InitializeComponent();
        KeyCodeComboBox.ItemsSource = KeyCodeKeys;
        KeyCodeComboBox.SelectedIndex = 0;
    }

    /// <summary>
    /// Set values
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="e"></param>
    private void ClosingHandler(ContentDialog dialog, ContentDialogClosingEventArgs e) {
        var value = KeyCodeKeys[KeyCodeComboBox.SelectedIndex];
        Parameters = new object[] { Enum.Parse<KeyCode>(value) };
        DescriptionValue = value;
    }
}
