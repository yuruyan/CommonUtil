using CommonUtil.Core;
using CommonUtil.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class RandomStringGeneratorView : Page, IGenerable<string> {
    public static readonly DependencyProperty CountListProperty = DependencyProperty.Register("CountList", typeof(List<int>), typeof(RandomStringGeneratorView), new PropertyMetadata());
    public static readonly DependencyProperty NumberCheckedProperty = DependencyProperty.Register("NumberChecked", typeof(bool), typeof(RandomStringGeneratorView), new PropertyMetadata(true));
    public static readonly DependencyProperty UppercaseCheckedProperty = DependencyProperty.Register("UppercaseChecked", typeof(bool), typeof(RandomStringGeneratorView), new PropertyMetadata(true));
    public static readonly DependencyProperty LowerCaseCheckedProperty = DependencyProperty.Register("LowerCaseChecked", typeof(bool), typeof(RandomStringGeneratorView), new PropertyMetadata(false));
    public static readonly DependencyProperty SpecialCharacterCheckedProperty = DependencyProperty.Register("SpecialCharacterChecked", typeof(bool), typeof(RandomStringGeneratorView), new PropertyMetadata(false));
    public static readonly DependencyProperty GenerateCountProperty = DependencyProperty.Register("GenerateCount", typeof(int), typeof(RandomStringGeneratorView), new PropertyMetadata(8));
    public static readonly DependencyProperty StringLengthProperty = DependencyProperty.Register("StringLength", typeof(int), typeof(RandomStringGeneratorView), new PropertyMetadata(8));

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
    /// 生成数量
    /// </summary>
    public int GenerateCount {
        get { return (int)GetValue(GenerateCountProperty); }
        set { SetValue(GenerateCountProperty, value); }
    }
    /// <summary>
    /// 字符长度
    /// </summary>
    public int StringLength {
        get { return (int)GetValue(StringLengthProperty); }
        set { SetValue(StringLengthProperty, value); }
    }
    /// <summary>
    /// 数字列表
    /// </summary>
    public List<int> CountList {
        get { return (List<int>)GetValue(CountListProperty); }
        set { SetValue(CountListProperty, value); }
    }

    public RandomStringGeneratorView() {
        CountList = new();
        for (int i = 1; i <= 100; i++) {
            CountList.Add(i);
        }
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate() {
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
        return RandomGenerator.GenerateRandomString(choice, StringLength, GenerateCount);
    }
}

