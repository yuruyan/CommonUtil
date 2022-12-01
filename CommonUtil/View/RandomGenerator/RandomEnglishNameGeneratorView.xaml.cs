using CommonUtil.Core.Model;

namespace CommonUtil.View;

public partial class RandomEnglishNameGeneratorView : Page, IGenerable<uint, IEnumerable<string>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public RandomEnglishNameGeneratorView() {
        InitializeComponent();
        // 提前加载，减少卡顿
        Task.Run(() => {
            TaskUtils.Try(() => RandomGenerator.GenerateRandomEnglishNames(1));
        });
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate(uint generateCount) {
        try {
            return RandomGenerator.GenerateRandomEnglishNames(generateCount);
        } catch (Exception e) {
            MessageBox.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
