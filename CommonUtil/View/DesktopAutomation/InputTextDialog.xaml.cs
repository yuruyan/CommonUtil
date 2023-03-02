using ModernWpf.Controls;

namespace CommonUtil.View;

public partial class InputTextDialog : DesktopAutomationDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(InputTextDialog), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 输入文本
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }

    public InputTextDialog() {
        AutomationMethod = DesktopAutomation.InputText;
        DescriptionHeader = "输入文本";
        InitializeComponent();
    }

    private void ClosingHandler(ContentDialog _, ContentDialogClosingEventArgs e) {
        Parameters = new object[] { InputText };
        DescriptionValue = InputText;
    }
}
