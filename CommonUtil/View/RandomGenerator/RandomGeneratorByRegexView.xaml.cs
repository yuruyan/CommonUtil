using CommonUITools.Utils;
using CommonUtil.Core;
using CommonUtil.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class RandomGeneratorByRegexView : Page, IGenerable<string> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty GenerateCountProperty = DependencyProperty.Register("GenerateCount", typeof(double), typeof(RandomGeneratorByRegexView), new PropertyMetadata(8.0));
    public static readonly DependencyProperty RegexInputTextProperty = DependencyProperty.Register("RegexInputText", typeof(string), typeof(RandomGeneratorByRegexView), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 生成个数
    /// </summary>
    public double GenerateCount {
        get { return (double)GetValue(GenerateCountProperty); }
        set { SetValue(GenerateCountProperty, value); }
    }
    /// <summary>
    /// 正则输入
    /// </summary>
    public string RegexInputText {
        get { return (string)GetValue(RegexInputTextProperty); }
        set { SetValue(RegexInputTextProperty, value); }
    }

    public RandomGeneratorByRegexView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate() {
        if (string.IsNullOrEmpty(RegexInputText)) {
            MessageBox.Info("正则表达式不能为空！");
            return Array.Empty<string>();
        }
        // 判断正则是否合法
        if (CommonUtils.Try(() => new Regex(RegexInputText)) is null) {
            MessageBox.Error("正则表达式无效！");
            return Array.Empty<string>();
        }
        try {
            return RandomGenerator.GenerateRandomStringWithRegex(RegexInputText, (int)GenerateCount);
        } catch (Exception e) {
            MessageBox.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}

