using CommonUITools.View;

namespace CommonUtil.View;

public partial class WaitDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public WaitDialog() {
        InitializeComponent();
    }
}
