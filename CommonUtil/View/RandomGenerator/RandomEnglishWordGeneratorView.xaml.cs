namespace CommonUtil.View;

public partial class RandomEnglishWordGeneratorView : RandomGeneratorPage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public RandomEnglishWordGeneratorView() {
        InitializeComponent();
        // 提前加载，减少卡顿
        Task.Run(() => {
            TaskUtils.Try(() => RandomGenerator.GenerateRandomEnglishWords(1));
        });
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<string> Generate(uint generateCount) {
        try {
            return RandomGenerator.GenerateRandomEnglishWords(generateCount);
        } catch (Exception e) {
            MessageBoxUtils.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
