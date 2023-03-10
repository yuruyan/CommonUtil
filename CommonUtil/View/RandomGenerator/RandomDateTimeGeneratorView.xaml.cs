namespace CommonUtil.View;

public partial class RandomDateTimeGeneratorView : Page, IGenerable<uint, IEnumerable<string>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty StartDateTimeProperty = DependencyProperty.Register("StartDateTime", typeof(DateTime), typeof(RandomDateTimeGeneratorView), new PropertyMetadata(new DateTime(DateTime.UtcNow.Year - 1, 1, 1)));
    public static readonly DependencyProperty EndDateTimeProperty = DependencyProperty.Register("EndDateTime", typeof(DateTime), typeof(RandomDateTimeGeneratorView), new PropertyMetadata(new DateTime(DateTime.UtcNow.Year + 1, 1, 1)));
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

    public RandomDateTimeGeneratorView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate(uint generateCount) {
        try {
            if (EndDateTime < StartDateTime) {
                throw new Exception("结束日期不能小于开始日期");
            }

            return RandomGenerator
                .GenerateRandomDateTime(StartDateTime, EndDateTime, generateCount)
                .Select(t => t.ToString());
        } catch (Exception e) {
            MessageBox.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
