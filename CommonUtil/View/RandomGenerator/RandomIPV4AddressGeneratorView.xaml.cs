using CommonUtil.Core.Model;

namespace CommonUtil.View;

public partial class RandomIPV4AddressGeneratorView : Page, IGenerable<uint, IEnumerable<string>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public RandomIPV4AddressGeneratorView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate(uint generateCount) {
        try {
            return RandomGenerator.GenerateRandomIPV4Addresses(generateCount);
        } catch (Exception e) {
            MessageBox.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
