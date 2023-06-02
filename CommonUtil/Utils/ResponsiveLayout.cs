namespace CommonUtil.Utils;

public sealed class ResponsiveLayout : DependencyObject {
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ResponsiveLayout), new PropertyMetadata(true, IsExpandedPropertyChangedHandler));
    public static readonly DependencyProperty ExpansionThresholdProperty = DependencyProperty.Register("ExpansionThreshold", typeof(double), typeof(ResponsiveLayout), new PropertyMetadata(0.0));
    private UIElement? ControlPanel;
    private double ExpandedWidth;
    public const string DefaultExpansionThresholdKey = "ExpansionThreshold";
    public const string DefaultControlPanelName = "ControlPanel";

    public bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }
    public double ExpansionThreshold {
        get { return (double)GetValue(ExpansionThresholdProperty); }
        set { SetValue(ExpansionThresholdProperty, value); }
    }
    public string ExpansionThresholdKey { get; init; } = DefaultExpansionThresholdKey;
    public string ControlPanelName { get; init; } = DefaultControlPanelName;
    /// <summary>
    /// If <see cref="ResponsiveMode.Fixed"/> is specified, <see cref="ExpansionThreshold"/> must be provided, which is retrieved by <see cref="ExpansionThresholdKey"/> only once;<br/>
    /// If <see cref="ResponsiveMode.Variable"/> is specified, <see cref="ControlPanel"/> must be provided, which is retrieved by <see cref="ControlPanelName"/> only once;<br/>
    /// Default value is <see cref="ResponsiveMode.Fixed"/>
    /// </summary>
    public ResponsiveMode ResponsiveMode { get; }
    public Action<ResponsiveLayout, DependencyPropertyChangedEventArgs> IsExpandedPropertyChanged { get; init; }
    public Action<FrameworkElement, SizeChangedEventArgs> ElementSizeChanged { get; }
    public FrameworkElement Element { get; }

    public ResponsiveLayout(
        FrameworkElement element,
        ResponsiveMode responsiveMode,
        Action<ResponsiveLayout, DependencyPropertyChangedEventArgs> isExpandedPropertyChanged,
        Action<FrameworkElement, SizeChangedEventArgs> elementSizeChanged
    ) {
        Element = element;
        ResponsiveMode = responsiveMode;
        this.IsExpandedPropertyChanged = isExpandedPropertyChanged;
        ElementSizeChanged = elementSizeChanged;
        element.Initialized += ElementInitializedHandler;
    }

    private void ElementInitializedHandler(object? _, EventArgs e) {
        if (ResponsiveMode == ResponsiveMode.Variable) {
            ControlPanel = Element.FindName(ControlPanelName) as UIElement;
            Element.SizeChanged += ElementSizeChangedVariableHandler;
        } else if (ResponsiveMode == ResponsiveMode.Fixed) {
            ExpansionThreshold = (double)Element.Resources[ExpansionThresholdKey];
            Element.SizeChanged += ElementSizeChangedFixedHandler;
        }
        Element.SizeChanged += ElementSizeChangedHandler;
    }

    /// <summary>
    /// Size changed handler for <see cref="ResponsiveMode.Fixed"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ElementSizeChangedFixedHandler(object sender, SizeChangedEventArgs e) {
        IsExpanded = ExpansionThreshold <= e.NewSize.Width;
    }

    /// <summary>
    /// Size changed handler for <see cref="ResponsiveMode.Variable"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ElementSizeChangedVariableHandler(object sender, SizeChangedEventArgs e) {
        if (ControlPanel is null) {
            return;
        }

        ControlPanel.Measure(new(short.MaxValue, short.MaxValue));
        if (IsExpanded) {
            ExpandedWidth = ControlPanel.DesiredSize.Width;
        }
        IsExpanded = ExpandedWidth <= e.NewSize.Width;
    }

    /// <summary>
    /// Size changed handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ElementSizeChangedHandler(object sender, SizeChangedEventArgs e) => ElementSizeChanged((FrameworkElement)sender, e);

    private static void IsExpandedPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is ResponsiveLayout self) {
            self.IsExpandedPropertyChanged(self, e);
        }
    }
}
