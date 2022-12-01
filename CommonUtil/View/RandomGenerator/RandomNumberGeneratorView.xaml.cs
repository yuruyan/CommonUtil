using CommonUtil.Core.Model;

namespace CommonUtil.View;

public partial class RandomNumberGeneratorView : Page, IGenerable<uint, IEnumerable<string>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(RandomNumberGeneratorView), new PropertyMetadata(1.0));
    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(RandomNumberGeneratorView), new PropertyMetadata(100.0));

    /// <summary>
    /// 最小值
    /// </summary>
    public double MinValue {
        get { return (double)GetValue(MinValueProperty); }
        set { SetValue(MinValueProperty, value); }
    }
    /// <summary>
    /// 最大值
    /// </summary>
    public double MaxValue {
        get { return (double)GetValue(MaxValueProperty); }
        set { SetValue(MaxValueProperty, value); }
    }

    public RandomNumberGeneratorView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    public IEnumerable<string> Generate(uint generateCount) {
        int minValue = (int)MinValue;
        int maxValue = (int)MaxValue;
        if (minValue > maxValue) {
            MessageBox.Error("数字范围无效");
            return Array.Empty<string>();
        }
        if (maxValue != int.MaxValue) {
            maxValue += 1;
        }
        return RandomGenerator
            .GenerateRandomNumber(minValue, maxValue, generateCount)
            .Select(n => n.ToString());
    }
}
