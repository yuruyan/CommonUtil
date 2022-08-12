using CommonUITools.Utils;
using CommonUtil.Core;
using CommonUtil.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CommonUtil.View;

public partial class RandomGeneratorWithDataSourceView : System.Windows.Controls.Page, IGenerable<string> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private const string DefaultDataSource = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static readonly DependencyProperty GenerateCountProperty = DependencyProperty.Register("GenerateCount", typeof(double), typeof(RandomGeneratorWithDataSourceView), new PropertyMetadata(16.0));
    public static readonly DependencyProperty MinStringLengthProperty = DependencyProperty.Register("MinStringLength", typeof(double), typeof(RandomGeneratorWithDataSourceView), new PropertyMetadata(8.0));
    public static readonly DependencyProperty MaxStringLengthProperty = DependencyProperty.Register("MaxStringLength", typeof(double), typeof(RandomGeneratorWithDataSourceView), new PropertyMetadata(16.0));
    public static readonly DependencyProperty DataSourceTextProperty = DependencyProperty.Register("DataSourceText", typeof(string), typeof(RandomGeneratorWithDataSourceView), new PropertyMetadata(DefaultDataSource));

    /// <summary>
    /// 生成数量
    /// </summary>
    public double GenerateCount {
        get { return (double)GetValue(GenerateCountProperty); }
        set { SetValue(GenerateCountProperty, value); }
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
    /// <summary>
    /// 数据源
    /// </summary>
    public string DataSourceText {
        get { return (string)GetValue(DataSourceTextProperty); }
        set { SetValue(DataSourceTextProperty, value); }
    }

    public RandomGeneratorWithDataSourceView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate() {
        Range? range = CommonUtils.CheckRange(MinStringLength, MaxStringLength);
        if (range is null) {
            CommonUITools.Widget.MessageBox.Error("字符串范围无效");
            return Array.Empty<string>();
        }
        // 检查数据源
        if (string.IsNullOrEmpty(DataSourceText)) {
            CommonUITools.Widget.MessageBox.Info("请输入数据源");
            return Array.Empty<string>();
        }
        return RandomGenerator.GenerateRandomStringWithDataSource(
            DataSourceText.ToCharArray(),
            new Range(range.Value.Start, new(range.Value.End.Value + 1)),
            (int)GenerateCount
        );
    }

}
