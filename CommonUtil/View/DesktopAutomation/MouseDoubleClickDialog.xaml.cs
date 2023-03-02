using CommonUITools.View;

namespace CommonUtil.View;

public partial class MouseDoubleClickDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public MouseDoubleClickDialog() {
        InitializeComponent();
    }
}
