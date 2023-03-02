using CommonUITools.View;

namespace CommonUtil.View;

public partial class PressKeyDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public PressKeyDialog() {
        InitializeComponent();
    }
}
