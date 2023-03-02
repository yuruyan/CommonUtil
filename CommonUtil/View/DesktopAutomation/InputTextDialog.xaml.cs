using CommonUITools.View;

namespace CommonUtil.View;

public partial class InputTextDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public InputTextDialog() {
        InitializeComponent();
    }
}
