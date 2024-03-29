﻿namespace CommonUtil.View;

public class ResponsiveUserControl : UserControl {
    private static readonly DependencyPropertyKey ResponsiveLayoutPropertyKey = DependencyProperty.RegisterReadOnly("ResponsiveLayout", typeof(ResponsiveLayout), typeof(ResponsiveUserControl), new PropertyMetadata());
    public static readonly DependencyProperty ResponsiveLayoutProperty = ResponsiveLayoutPropertyKey.DependencyProperty;

    public ResponsiveLayout ResponsiveLayout => (ResponsiveLayout)GetValue(ResponsiveLayoutProperty);

    public ResponsiveUserControl() : this(ResponsiveMode.Fixed) { }

    public ResponsiveUserControl(
        ResponsiveMode responsiveMode,
        string expansionThresholdKey = ResponsiveLayout.DefaultExpansionThresholdKey,
        string controlPanelName = ResponsiveLayout.DefaultControlPanelName
    ) {
        var layout = new ResponsiveLayout(
            this,
            responsiveMode,
            IsExpandedPropertyChangedHandler
        ) {
            ExpansionThresholdKey = expansionThresholdKey,
            ControlPanelName = controlPanelName
        };
        SetValue(ResponsiveLayoutPropertyKey, layout);
    }

    /// <summary>
    /// IsExpanded Changed
    /// </summary>
    /// <param name="self"></param>
    /// <param name="e"></param>
    protected virtual void IsExpandedPropertyChangedHandler(ResponsiveLayout self, DependencyPropertyChangedEventArgs e) { }
}
