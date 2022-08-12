using CommonUtil.Core;
using CommonUtil.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View;

public partial class RandomNumberGeneratorView : Page, IGenerable<IEnumerable<string>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(RandomNumberGeneratorView), new PropertyMetadata(1.0));
    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(RandomNumberGeneratorView), new PropertyMetadata(100.0));
    public static readonly DependencyProperty GenerateCountProperty = DependencyProperty.Register("GenerateCount", typeof(double), typeof(RandomNumberGeneratorView), new PropertyMetadata(16.0));

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
    /// <summary>
    /// 生成个数
    /// </summary>
    public double GenerateCount {
        get { return (double)GetValue(GenerateCountProperty); }
        set { SetValue(GenerateCountProperty, value); }
    }

    public RandomNumberGeneratorView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    public IEnumerable<string> Generate() {
        int minValue = (int)MinValue;
        int maxValue = (int)MaxValue;
        if (minValue > maxValue) {
            CommonUITools.Widget.MessageBox.Error("数字范围无效");
            return Array.Empty<string>();
        }
        if (maxValue != int.MaxValue) {
            maxValue += 1;
        }
        return RandomGenerator
            .GenerateRandomNumber(minValue, maxValue, (int)GenerateCount)
            .Select(n => n.ToString());
    }
}
