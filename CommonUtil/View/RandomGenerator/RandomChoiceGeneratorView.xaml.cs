using CommonUtil.Resources;

namespace CommonUtil.View;

public partial class RandomChoiceGeneratorView : RandomGeneratorPage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly string DefaultDataSource = string.Join('\n', Resource.RandomChoiceGeneratorDefaultDataSource.ReplaceLineFeedWithLinuxStyle().Split('\n'));
    public static readonly DependencyProperty DataSourceTextProperty = DependencyProperty.Register("DataSourceText", typeof(string), typeof(RandomChoiceGeneratorView), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 数据源
    /// </summary>
    public string DataSourceText {
        get { return (string)GetValue(DataSourceTextProperty); }
        set { SetValue(DataSourceTextProperty, value); }
    }

    public RandomChoiceGeneratorView() {
        DataSourceText = DefaultDataSource;
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<string> Generate(uint generateCount) {
        // 检查数据源
        if (string.IsNullOrEmpty(DataSourceText)) {
            MessageBoxUtils.Info("请输入数据源");
            return Array.Empty<string>();
        }
        return RandomGenerator.GenerateRandomStringChoices(
            DataSourceText.ReplaceLineFeedWithLinuxStyle().Split('\n'),
            generateCount
        );
    }
}
