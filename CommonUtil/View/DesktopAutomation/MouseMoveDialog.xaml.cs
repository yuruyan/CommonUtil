using CommonUITools.View;

namespace CommonUtil.View;

public partial class MouseMoveDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public MouseMoveDialog() {
        InitializeComponent();
    }
}
