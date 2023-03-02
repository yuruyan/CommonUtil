using ModernWpf.Controls;
using WindowsInput.Events;

namespace CommonUtil.View;

public partial class PressKeyShortcutDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public PressKeyShortcutDialog() {
        AutomationMethod = DesktopAutomation.PressKeyShortcut;
        DescriptionHeader = "按下快捷键";
        InitializeComponent();
    }

    /// <summary>
    /// Set values
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="e"></param>
    private void ClosingHandler(ContentDialog dialog, ContentDialogClosingEventArgs e) {
        Parameters = new object[] {
            new KeyCode[] { KeyCode.None }
        };
        DescriptionValue = "";
    }
}
