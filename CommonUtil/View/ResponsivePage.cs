namespace CommonUtil.View;

public class ResponsivePage : Page, IResponsiveElement {
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ResponsivePage), new PropertyMetadata(true, IsExpandedPropertyChangedHandler));
    public static readonly DependencyProperty ExpansionThresholdProperty = DependencyProperty.Register("ExpansionThreshold", typeof(double), typeof(ResponsivePage), new PropertyMetadata(0.0));

    public virtual bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }
    public virtual double ExpansionThreshold {
        get { return (double)GetValue(ExpansionThresholdProperty); }
        set { SetValue(ExpansionThresholdProperty, value); }
    }
    public virtual string ExpansionThresholdKey { get; set; } = "ExpansionThreshold";

    protected override void OnInitialized(EventArgs e) {
        base.OnInitialized(e);
        // 自动设置值
        if (ExpansionThresholdProperty.GetMetadata(this).DefaultValue == GetValue(ExpansionThresholdProperty)) {
            if (Resources[ExpansionThresholdKey] is double value) {
                ExpansionThreshold = value;
            }
        }
        IsExpanded = ActualWidth >= ExpansionThreshold;
        SizeChanged += ElementSizeChangedHandler;
    }

    /// <summary>
    /// Size Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void ElementSizeChangedHandler(object sender, SizeChangedEventArgs e) {
        IsExpanded = e.NewSize.Width >= ExpansionThreshold;
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
