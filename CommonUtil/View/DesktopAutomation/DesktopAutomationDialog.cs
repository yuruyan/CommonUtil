using CommonUITools.View;

namespace CommonUtil.View;

public abstract class DesktopAutomationDialog : BaseDialog {
    public static readonly DependencyProperty AutomationMethodProperty = DependencyProperty.Register("AutomationMethod", typeof(Delegate), typeof(DesktopAutomationDialog), new PropertyMetadata());
    public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(object[]), typeof(DesktopAutomationDialog), new PropertyMetadata());
    public static readonly DependencyProperty DescriptionHeaderProperty = DependencyProperty.Register("DescriptionHeader", typeof(string), typeof(DesktopAutomationDialog), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty DescriptionValueProperty = DependencyProperty.Register("DescriptionValue", typeof(string), typeof(DesktopAutomationDialog), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 调用方法
    /// </summary>
    public Delegate AutomationMethod {
        get { return (Delegate)GetValue(AutomationMethodProperty); }
        protected set { SetValue(AutomationMethodProperty, value); }
    }
    /// <summary>
    /// 方法参数
    /// </summary>
    public object[] Parameters {
        get { return (object[])GetValue(ParametersProperty); }
        set { SetValue(ParametersProperty, value); }
    }
    /// <summary>
    /// 描述信息头
    /// </summary>
    public string DescriptionHeader {
        get { return (string)GetValue(DescriptionHeaderProperty); }
        set { SetValue(DescriptionHeaderProperty, value); }
    }
    /// <summary>
    /// 描述信息值
    /// </summary>
    public string DescriptionValue {
        get { return (string)GetValue(DescriptionValueProperty); }
        set { SetValue(DescriptionValueProperty, value); }
    }

    /// <summary>
    /// 解析参数
    /// </summary>
    /// <param name="parameters"></param>
    public abstract void ParseParameters(object[] parameters);
}
