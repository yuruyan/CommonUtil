using CommonUtil.Core.Model;

namespace CommonUtil.View;

public partial class RandomStringGeneratorView : Page, IGenerable<uint, IEnumerable<string>> {
    public static readonly DependencyProperty NumberCheckedProperty = DependencyProperty.Register("NumberChecked", typeof(bool), typeof(RandomStringGeneratorView), new PropertyMetadata(true));
    public static readonly DependencyProperty UppercaseCheckedProperty = DependencyProperty.Register("UppercaseChecked", typeof(bool), typeof(RandomStringGeneratorView), new PropertyMetadata(true));
    public static readonly DependencyProperty LowerCaseCheckedProperty = DependencyProperty.Register("LowerCaseChecked", typeof(bool), typeof(RandomStringGeneratorView), new PropertyMetadata(false));
    public static readonly DependencyProperty SpecialCharacterCheckedProperty = DependencyProperty.Register("SpecialCharacterChecked", typeof(bool), typeof(RandomStringGeneratorView), new PropertyMetadata(false));
    public static readonly DependencyProperty MinStringLengthProperty = DependencyProperty.Register("MinStringLength", typeof(double), typeof(RandomStringGeneratorView), new PropertyMetadata(8.0));
    public static readonly DependencyProperty MaxStringLengthProperty = DependencyProperty.Register("MaxStringLength", typeof(double), typeof(RandomStringGeneratorView), new PropertyMetadata(16.0));

    /// <summary>
    /// 数字选中
    /// </summary>
    public bool NumberChecked {
        get { return (bool)GetValue(NumberCheckedProperty); }
        set { SetValue(NumberCheckedProperty, value); }
    }
    /// <summary>
    /// 小写字母选中
    /// </summary>
    public bool LowerCaseChecked {
        get { return (bool)GetValue(LowerCaseCheckedProperty); }
        set { SetValue(LowerCaseCheckedProperty, value); }
    }
    /// <summary>
    /// 大写字母选中
    /// </summary>
    public bool UppercaseChecked {
        get { return (bool)GetValue(UppercaseCheckedProperty); }
        set { SetValue(UppercaseCheckedProperty, value); }
    }
    /// <summary>
    /// 特殊字符选中
    /// </summary>
    public bool SpecialCharacterChecked {
        get { return (bool)GetValue(SpecialCharacterCheckedProperty); }
        set { SetValue(SpecialCharacterCheckedProperty, value); }
    }
    /// <summary>
    /// 字符串最小长度
    /// </summary>
    public double MinStringLength {
        get { return (double)GetValue(MinStringLengthProperty); }
        set { SetValue(MinStringLengthProperty, value); }
    }
    /// <summary>
    /// 字符串最大长度
    /// </summary>
    public double MaxStringLength {
        get { return (double)GetValue(MaxStringLengthProperty); }
        set { SetValue(MaxStringLengthProperty, value); }
    }

    public RandomStringGeneratorView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate(uint generateCount) {
        Range? range = CommonUtils.CheckRange(MinStringLength, MaxStringLength);
        if (range is null) {
            MessageBox.Error("字符串范围无效");
            return Array.Empty<string>();
        }
        var choice = RandomStringChoice.None;
        if (NumberChecked) {
            choice |= RandomStringChoice.Number;
        }
        if (LowerCaseChecked) {
            choice |= RandomStringChoice.LowerCase;
        }
        if (UppercaseChecked) {
            choice |= RandomStringChoice.UpperCase;
        }
        if (SpecialCharacterChecked) {
            choice |= RandomStringChoice.SpacialCharacter;
        }
        return RandomGenerator.GenerateRandomString(
            choice,
            new Range(range.Value.Start, new(range.Value.End.Value + 1)),
            generateCount
        );
    }

}
