namespace CommonUtil.Model;

public class AutomationItem {
    public uint Id { get; }
    public string Name { get; }
    public string Icon { get; }
    public bool IsFolder { get; }
    public IReadOnlyList<AutomationItem> Children { get; init; } = new List<AutomationItem>();

    public AutomationItem(string name, string icon, uint id = 0, bool isFolder = false) {
        Id = id;
        Name = name;
        Icon = icon;
        IsFolder = isFolder;
    }
}

public class AutomationStep : DependencyObject {
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(AutomationStep), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty DescriptionHeaderProperty = DependencyProperty.Register("DescriptionHeader", typeof(string), typeof(AutomationStep), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty DescriptionValueProperty = DependencyProperty.Register("DescriptionValue", typeof(string), typeof(AutomationStep), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon {
        get { return (string)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    /// <summary>
    /// 调用方法
    /// </summary>
    public Delegate AutomationMethod { get; }
    /// <summary>
    /// 方法参数
    /// </summary>
    public object[]? Parameters { get; }
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

    public AutomationStep(Delegate automationMethod, object[]? parameters) {
        AutomationMethod = automationMethod;
        Parameters = parameters;
    }
}