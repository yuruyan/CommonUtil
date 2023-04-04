namespace CommonUtil.View;

public partial class RandomJapaneseNameGeneratorView : RandomGeneratorPage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public RandomJapaneseNameGeneratorView() {
        InitializeComponent();
        // 提前加载，减少卡顿
        Task.Run(() => {
            TaskUtils.Try(() => RandomGenerator.GenerateRandomJapaneseNames(1));
        });
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<string> Generate(uint generateCount) {
        try {
            return RandomGenerator.GenerateRandomJapaneseNames(generateCount);
        } catch (Exception e) {
            MessageBoxUtils.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
