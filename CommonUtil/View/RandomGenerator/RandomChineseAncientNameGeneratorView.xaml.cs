﻿using CommonUITools.Utils;
using CommonUtil.Core;
using CommonUtil.Core.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class RandomChineseAncientNameGeneratorView : Page, IGenerable<uint, IEnumerable<string>> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public RandomChineseAncientNameGeneratorView() {
        InitializeComponent();
        // 提前加载，减少卡顿
        Task.Run(() => {
            TaskUtils.Try(() => RandomGenerator.GenerateRandomChineseAncientNames(1));
        });
    }

    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Generate(uint generateCount) {
        try {
            return RandomGenerator.GenerateRandomChineseAncientNames(generateCount);
        } catch (Exception e) {
            MessageBox.Error($"生成失败：{e.Message}");
            return Array.Empty<string>();
        }
    }
}
