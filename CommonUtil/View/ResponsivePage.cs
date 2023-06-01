namespace CommonUtil.View;

/// <summary>
/// 如果重写 OnInitialized 方法，则注意调用 "base.OnInitialized(e);"<br/>
/// 设置 Panel 的 Name 属性为 <see cref="ResponsivePage.ControlPanelName"/>
/// </summary>
public class ResponsivePage : Page {
    protected static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ResponsivePage), new PropertyMetadata(true, IsExpandedPropertyChangedHandler));
    protected static readonly DependencyProperty ExpansionThresholdProperty = DependencyProperty.Register("ExpansionThreshold", typeof(double), typeof(ResponsivePage), new PropertyMetadata(0.0));
    private UIElement? ControlPanel;
    private double ExpandedWidth;

    protected virtual bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }
    protected virtual double ExpansionThreshold {
        get { return (double)GetValue(ExpansionThresholdProperty); }
        set { SetValue(ExpansionThresholdProperty, value); }
    }
    protected virtual string ExpansionThresholdKey { get; set; } = "ExpansionThreshold";
    protected virtual string ControlPanelName { get; set; } = "ControlPanel";

    protected override void OnInitialized(EventArgs e) {
        base.OnInitialized(e);
        ControlPanel = FindName(ControlPanelName) as UIElement;
        SizeChanged += ElementSizeChangedHandler;
    }

    /// <summary>
    /// Size Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void ElementSizeChangedHandler(object sender, SizeChangedEventArgs e) {
        if (ControlPanel is null) {
            return;
        }

        if (IsExpanded) {
            ExpandedWidth = ControlPanel.RenderSize.Width;
        }
        IsExpanded = ExpandedWidth <= e.NewSize.Width;
    }

    /// <summary>
    /// IsExpanded Changed
    /// </summary>
    /// <param name="self"></param>
    /// <param name="e"></param>
    protected virtual void IsExpandedPropertyChangedHandler(ResponsivePage self, DependencyPropertyChangedEventArgs e) { }

    private static void IsExpandedPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is ResponsivePage self) {
            self.IsExpandedPropertyChangedHandler(self, e);
        }
    }
}
