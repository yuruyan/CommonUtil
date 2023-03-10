namespace CommonUtil.View;

public partial class RandomMACAddressGeneratorView : Page, IGenerable<uint, IEnumerable<string>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// 是否是大写
    /// </summary>
    public bool IsUpperCase {
        get { return (bool)GetValue(IsUpperCaseProperty); }
        set { SetValue(IsUpperCaseProperty, value); }
    }
    public static readonly DependencyProperty IsUpperCaseProperty = DependencyProperty.Register("IsUpperCase", typeof(bool), typeof(RandomMACAddressGeneratorView), new PropertyMetadata(false));

    public RandomMACAddressGeneratorView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate(uint generateCount) {
        try {
            return RandomGenerator.GenerateRandomMACAddresses(generateCount, IsUpperCase);
        } catch (Exception e) {
            MessageBox.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
