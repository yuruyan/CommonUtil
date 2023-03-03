using ModernWpf.Controls;
using WindowsInput.Events;

namespace CommonUtil.View;

public partial class PressKeyDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly IList<string> KeyCodeKeys = Enum.GetNames<KeyCode>();

    public PressKeyDialog() {
        AutomationMethod = DesktopAutomation.PressKey;
        Title = DescriptionHeader = "按下键盘";
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
        _ = dialog;
        _ = e;
        var value = KeyCodeKeys[KeyCodeComboBox.SelectedIndex];
        Parameters = new object[] { Enum.Parse<KeyCode>(value) };
        DescriptionValue = value;
    }

    public override void ParseParameters(object[] parameters) {
        KeyCodeComboBox.SelectedIndex = KeyCodeKeys.IndexOf(((KeyCode)parameters[0]).ToString());
    }
}
