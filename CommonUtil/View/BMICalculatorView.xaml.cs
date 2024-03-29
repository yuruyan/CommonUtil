﻿namespace CommonUtil.View;

public partial class BMICalculatorView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty HeightTextProperty = DependencyProperty.Register("HeightText", typeof(string), typeof(BMICalculatorView), new PropertyMetadata(""));
    public static readonly DependencyProperty WeightTextProperty = DependencyProperty.Register("WeightText", typeof(string), typeof(BMICalculatorView), new PropertyMetadata(""));
    public static readonly DependencyProperty BMIProperty = DependencyProperty.Register("BMI", typeof(string), typeof(BMICalculatorView), new PropertyMetadata(""));
    public static readonly DependencyProperty HealthProperty = DependencyProperty.Register("Health", typeof(string), typeof(BMICalculatorView), new PropertyMetadata(""));
    public static readonly DependencyProperty HealthColorProperty = DependencyProperty.Register("HealthColor", typeof(string), typeof(BMICalculatorView), new PropertyMetadata("#DBDBDB"));

    /// <summary>
    /// 身高
    /// </summary>
    public string HeightText {
        get { return (string)GetValue(HeightTextProperty); }
        set { SetValue(HeightTextProperty, value); }
    }
    /// <summary>
    /// 体重
    /// </summary>
    public string WeightText {
        get { return (string)GetValue(WeightTextProperty); }
        set { SetValue(WeightTextProperty, value); }
    }
    /// <summary>
    /// BMI
    /// </summary>
    public string BMI {
        get { return (string)GetValue(BMIProperty); }
        set { SetValue(BMIProperty, value); }
    }
    /// <summary>
    /// 身体状态
    /// </summary>
    public string Health {
        get { return (string)GetValue(HealthProperty); }
        set { SetValue(HealthProperty, value); }
    }
    /// <summary>
    /// 身体状态颜色
    /// </summary>
    public string HealthColor {
        get { return (string)GetValue(HealthColorProperty); }
        set { SetValue(HealthColorProperty, value); }
    }

    public BMICalculatorView() {
        InitializeComponent();
    }

    /// <summary>
    /// 计算 bmi
    /// </summary>
    private void CalculateBMI() {
        double height = 0, weight = 0;
        try {
            height = Convert.ToDouble(HeightText) / 100;
            weight = Convert.ToDouble(WeightText);
        } catch {
            MessageBoxUtils.Error("输入有误！");
            return;
        }
        // 检查身高体重
        if (height <= 0 || weight <= 0) {
            MessageBoxUtils.Error("身高或体重有误！");
            return;
        }

        var bmi = BMICalculator.GetBMI(height, weight);
        BMI = $"{bmi:f2}";
        Health = bmi switch {
            < 18.5 => "偏瘦",
            < 24 => "正常",
            < 28 => "过重",
            >= 28 => "肥胖",
            _ => ""
        };
        HealthColor = bmi switch {
            < 18.5 => "#BBBBBB",
            < 24 => "#54a800",
            < 28 => "#e4a000",
            >= 28 => "#FF6E4C",
            _ => ""
        };
    }

    /// <summary>
    /// 计算 BMI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CalculateBMIClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        CalculateBMI();
    }

    /// <summary>
    /// 输入键盘事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextInputKeyUp(object sender, KeyEventArgs e) {
        e.Handled = true;
        if (e.Key == Key.Enter) {
            CalculateBMI();
        }
    }
}
