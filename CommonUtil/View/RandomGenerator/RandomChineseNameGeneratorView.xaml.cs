namespace CommonUtil.View;

public partial class RandomChineseNameGeneratorView : Page, IGenerable<uint, IEnumerable<string>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public RandomChineseNameGeneratorView() {
        InitializeComponent();
        // 提前加载，减少卡顿
        Task.Run(() => {
            TaskUtils.Try(() => RandomGenerator.GenerateRandomChineseNames(1));
        });
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate(uint generateCount) {
        try {
            return RandomGenerator.GenerateRandomChineseNames(generateCount);
        } catch (Exception e) {
            MessageBoxUtils.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
