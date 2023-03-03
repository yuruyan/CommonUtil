using ModernWpf.Controls;

namespace CommonUtil.View;

public partial class WaitDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty WaitTimeProperty = DependencyProperty.Register("WaitTime", typeof(double), typeof(WaitDialog), new PropertyMetadata(1000.0));

    /// <summary>
    /// 等待时间
    /// </summary>
    public double WaitTime {
        get { return (double)GetValue(WaitTimeProperty); }
        set { SetValue(WaitTimeProperty, value); }
    }

    public WaitDialog() {
        AutomationMethod = DesktopAutomation.Wait;
        Title = DescriptionHeader = "等待";
        InitializeComponent();
    }

    /// <summary>
    /// Set values
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="e"></param>
    private void ClosingHandler(ContentDialog dialog, ContentDialogClosingEventArgs e) {
        Parameters = new object[] { (uint)WaitTime };
        DescriptionValue = $"{(uint)WaitTime} ms";
    }

    public override void ParseParameters(object[] parameters) {
        WaitTime = (uint)parameters[0];
    }
}
