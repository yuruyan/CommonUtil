namespace CommonUtil.View;

public partial class RandomDateTimeGeneratorView : RandomGeneratorPage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty StartDateTimeProperty = DependencyProperty.Register("StartDateTime", typeof(DateTime), typeof(RandomDateTimeGeneratorView), new PropertyMetadata(new DateTime(DateTime.UtcNow.Year - 1, 1, 1)));
    public static readonly DependencyProperty EndDateTimeProperty = DependencyProperty.Register("EndDateTime", typeof(DateTime), typeof(RandomDateTimeGeneratorView), new PropertyMetadata(new DateTime(DateTime.UtcNow.Year + 1, 1, 1)));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RandomDateTimeGeneratorView), new PropertyMetadata(true));
    private const string ExpansionThresholdKey = "ExpansionThreshold";
    private readonly double ExpansionThreshold;

    /// <summary>
    /// 起始日期时间
    /// </summary>
    public DateTime StartDateTime {
        get { return (DateTime)GetValue(StartDateTimeProperty); }
        set { SetValue(StartDateTimeProperty, value); }
    }
    /// <summary>
    /// 终止日期时间
    /// </summary>
    public DateTime EndDateTime {
        get { return (DateTime)GetValue(EndDateTimeProperty); }
        set { SetValue(EndDateTimeProperty, value); }
    }
    /// <summary>
    /// 是否扩展
    /// </summary>
    public bool IsExpanded {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }

    public RandomDateTimeGeneratorView() {
        InitializeComponent();
        ExpansionThreshold = (double)Resources[ExpansionThresholdKey];
        SizeChanged += ViewSizeChangedHandler;
    }

    private void ViewSizeChangedHandler(object sender, SizeChangedEventArgs e) {
        IsExpanded = e.NewSize.Width >= ExpansionThreshold;
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<string> Generate(uint generateCount) {
        try {
            return EndDateTime < StartDateTime
                ? throw new Exception("结束日期不能小于开始日期")
                : RandomGenerator
                .GenerateRandomDateTime(StartDateTime, EndDateTime, generateCount)
                .Select(t => t.ToString());
        } catch (Exception e) {
            MessageBoxUtils.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
