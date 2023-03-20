namespace CommonUtil.View;

public class ResponsivePage : Page {
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ResponsivePage), new PropertyMetadata(true, IsExpandedPropertyChangedHandler));
    public static readonly DependencyProperty ExpansionThresholdProperty = DependencyProperty.Register("ExpansionThreshold", typeof(double), typeof(ResponsivePage), new PropertyMetadata(0.0));

    /// <summary>
    /// 是否扩展
    /// </summary>
    public bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }
    /// <summary>
    /// 扩展阈值
    /// </summary>
    public double ExpansionThreshold {
        get { return (double)GetValue(ExpansionThresholdProperty); }
        set { SetValue(ExpansionThresholdProperty, value); }
    }

    public ResponsivePage() {
        // 响应式布局
        this.SetLoadedOnceEventHandler(static (sender, _) => {
            if (sender is not ResponsivePage self) {
                return;
            }
            self.IsExpanded = self.ActualWidth >= self.ExpansionThreshold;
            self.SizeChanged += self.PageSizeChangedHandler;
        });
    }

    /// <summary>
    /// PageSize Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void PageSizeChangedHandler(object sender, SizeChangedEventArgs e) {
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
