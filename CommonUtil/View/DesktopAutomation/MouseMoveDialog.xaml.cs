using ModernWpf.Controls;

namespace CommonUtil.View;

public partial class MouseMoveDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty XPositionProperty = DependencyProperty.Register("XPosition", typeof(double), typeof(MouseMoveDialog), new PropertyMetadata(0.0));
    public static readonly DependencyProperty YPositionProperty = DependencyProperty.Register("YPosition", typeof(double), typeof(MouseMoveDialog), new PropertyMetadata(0.0));

    /// <summary>
    /// X 坐标
    /// </summary>
    public double XPosition {
        get { return (double)GetValue(XPositionProperty); }
        set { SetValue(XPositionProperty, value); }
    }
    /// <summary>
    /// Y 坐标
    /// </summary>
    public double YPosition {
        get { return (double)GetValue(YPositionProperty); }
        set { SetValue(YPositionProperty, value); }
    }

    public MouseMoveDialog() {
        AutomationMethod = DesktopAutomation.MouseMove;
        DescriptionHeader = "移动鼠标";
        InitializeComponent();
    }

    /// <summary>
    /// Set values
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="e"></param>
    private void ClosingHandler(ContentDialog dialog, ContentDialogClosingEventArgs e) {
        Parameters = new object[] { new Point(XPosition, YPosition) };
        DescriptionValue = $"({(int)XPosition}, {(int)YPosition})";
    }
}
