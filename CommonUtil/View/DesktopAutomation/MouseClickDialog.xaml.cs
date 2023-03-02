using CommonUITools.View;

namespace CommonUtil.View;

public partial class MouseClickDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public MouseClickDialog() {
        InitializeComponent();
    }
}
