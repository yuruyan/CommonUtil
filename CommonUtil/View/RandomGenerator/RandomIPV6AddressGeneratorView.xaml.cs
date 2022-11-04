using CommonUtil.Core;
using CommonUtil.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class RandomIPV6AddressGeneratorView : Page, IGenerable<uint, IEnumerable<string>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// 是否是大写
    /// </summary>
    public bool IsUpperCase {
        get { return (bool)GetValue(IsUpperCaseProperty); }
        set { SetValue(IsUpperCaseProperty, value); }
    }
    public static readonly DependencyProperty IsUpperCaseProperty = DependencyProperty.Register("IsUpperCase", typeof(bool), typeof(RandomIPV6AddressGeneratorView), new PropertyMetadata(false));

    public RandomIPV6AddressGeneratorView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate(uint generateCount) {
        try {
            return RandomGenerator.GenerateRandomIPV6Addresses(generateCount, IsUpperCase);
        } catch (Exception e) {
            MessageBox.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
