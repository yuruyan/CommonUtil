namespace CommonUtil.View;

public partial class RandomEmailAddressGeneratorView : RandomGeneratorPage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public RandomEmailAddressGeneratorView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<string> Generate(uint generateCount) {
        try {
            return RandomGenerator.GenerateRandomEmailAddresses(generateCount);
        } catch (Exception e) {
            MessageBoxUtils.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
