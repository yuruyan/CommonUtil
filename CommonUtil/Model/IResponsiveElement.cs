namespace CommonUtil.View;

/// <summary>
/// 响应式元素，需要在 <see cref="FrameworkElement.Resources"/> 设置 <see cref="ExpansionThresholdKey"/>, 
/// 并适当重写 IsExpandedPropertyChangedHandler 方法
/// </summary>
public interface IResponsiveElement {
    /// <summary>
    /// 是否扩展
    /// </summary>
    public bool IsExpanded { get; set; }
    /// <summary>
    /// 扩展阈值
    /// </summary>
    public double ExpansionThreshold { get; set; }
    /// <summary>
    /// Resources ExpansionThresholdKey
    /// </summary>
    public string ExpansionThresholdKey { get; set; }
}
