namespace CommonUtil.View;

public class ResponsiveUserControl : UserControl {
    protected ResponsiveLayout ResponsiveLayout { get; }

    public ResponsiveUserControl() : this(ResponsiveMode.Fixed) { }

    public ResponsiveUserControl(
        ResponsiveMode responsiveMode,
        string expansionThresholdKey = ResponsiveLayout.DefaultExpansionThresholdKey,
        string controlPanelName = ResponsiveLayout.DefaultControlPanelName
    ) {
        ResponsiveLayout = new(
            this,
            responsiveMode,
            IsExpandedPropertyChangedHandler,
            ElementSizeChangedHandler
        ) {
            ExpansionThresholdKey = expansionThresholdKey,
            ControlPanelName = controlPanelName
        };
    }

    /// <summary>
    /// Size changed handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void ElementSizeChangedHandler(object sender, SizeChangedEventArgs e) { }

    /// <summary>
    /// IsExpanded Changed
    /// </summary>
    /// <param name="self"></param>
    /// <param name="e"></param>
    protected virtual void IsExpandedPropertyChangedHandler(ResponsiveLayout self, DependencyPropertyChangedEventArgs e) { }
}
