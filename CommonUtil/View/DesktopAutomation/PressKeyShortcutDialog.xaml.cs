using CommonUITools.View;

namespace CommonUtil.View;

public partial class PressKeyShortcutDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public PressKeyShortcutDialog() {
        InitializeComponent();
    }
}
