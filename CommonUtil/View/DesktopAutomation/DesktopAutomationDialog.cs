using CommonUITools.View;

namespace CommonUtil.View;

public class DesktopAutomationDialog : BaseDialog {
    public static readonly DependencyProperty AutomationMethodProperty = DependencyProperty.Register("AutomationMethod", typeof(Delegate), typeof(DesktopAutomationDialog), new PropertyMetadata());
    public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(object[]), typeof(DesktopAutomationDialog), new PropertyMetadata());

    /// <summary>
    /// 调用方法
    /// </summary>
    public Delegate AutomationMethod {
        get { return (Delegate)GetValue(AutomationMethodProperty); }
        set { SetValue(AutomationMethodProperty, value); }
    }
    /// <summary>
    /// 方法参数
    /// </summary>
    public object[] Parameters {
        get { return (object[])GetValue(ParametersProperty); }
        set { SetValue(ParametersProperty, value); }
    }
}
