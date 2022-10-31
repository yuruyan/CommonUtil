using CommonUtil.Core;
using CommonUtil.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class RandomJapaneseFamilyNameGeneratorView : Page, IGenerable<uint, IEnumerable<string>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public RandomJapaneseFamilyNameGeneratorView() {
        InitializeComponent();
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate(uint generateCount) {
        try {
            return RandomGenerator.GenerateRandomJapaneseFamilyNames(generateCount);
        } catch (Exception e) {
            MessageBox.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
