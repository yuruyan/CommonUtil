﻿using ModernWpf.Controls;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CommonUtil.View;

public partial class CommonRegexListDialog : ContentDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly IEnumerable<KeyValuePair<string, string>> CommonRegexList;

    /// <summary>
    /// 加载 CommonRegexList
    /// </summary>
    static CommonRegexListDialog() {
        var list = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(
            Encoding.UTF8.GetString(Resource.Resource.CommonRegex)
        );
        if (list == null) {
            throw new JsonSerializationException("解析 CommonRegexList 失败");
        }
        CommonRegexList = list;
    }

    /// <summary>
    /// 正则列表
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> RegexList {
        get { return (IEnumerable<KeyValuePair<string, string>>)GetValue(RegexListProperty); }
        set { SetValue(RegexListProperty, value); }
    }
    public static readonly DependencyProperty RegexListProperty = DependencyProperty.Register("RegexList", typeof(IEnumerable<KeyValuePair<string, string>>), typeof(CommonRegexListDialog), new PropertyMetadata());

    public CommonRegexListDialog() {
        RegexList = CommonRegexList;
        InitializeComponent();
    }
}
