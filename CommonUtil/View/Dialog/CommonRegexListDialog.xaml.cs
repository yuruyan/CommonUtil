using CommonUITools.Utils;
using ModernWpf.Controls;
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
    /// 正则列表
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> RegexList {
        get { return (IEnumerable<KeyValuePair<string, string>>)GetValue(RegexListProperty); }
        set { SetValue(RegexListProperty, value); }
    }
    public static readonly DependencyProperty RegexListProperty = DependencyProperty.Register("RegexList", typeof(IEnumerable<KeyValuePair<string, string>>), typeof(CommonRegexListDialog), new PropertyMetadata());
    private readonly double RegexListItemsControlMinWidth;
    private readonly double RegexListItemsControlMaxWidth;

    /// <summary>
    /// 加载 CommonRegexList
    /// </summary>
    static CommonRegexListDialog() {
        var list = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(
            Encoding.UTF8.GetString(CommonUtil.Core.Resource.Resource.CommonRegex)
        );
        if (list == null) {
            throw new JsonSerializationException("解析 CommonRegexList 失败");
        }
        CommonRegexList = list;
    }

    public CommonRegexListDialog() {
        RegexList = CommonRegexList;
        InitializeComponent();
        RegexListItemsControlMinWidth = (double)Resources["RegexListItemsControlMinWidth"];
        RegexListItemsControlMaxWidth = (double)Resources["RegexListItemsControlMaxWidth"];
        // 动态更新 width
        UIUtils.SetLoadedOnceEventHandler(this, (_, _) => {
            var window = Window.GetWindow(this);
            UpdateDialogWidth(window.ActualWidth / 2);
            window.SizeChanged += (_, arg) => UpdateDialogWidth(arg.NewSize.Width / 2);
        });
    }

    /// <summary>
    /// 更新 RegexListItemsControl Width
    /// </summary>
    /// <param name="newWidth"></param>
    private void UpdateDialogWidth(double newWidth) {
        if (newWidth < RegexListItemsControlMinWidth) {
            newWidth = RegexListItemsControlMinWidth;
        } else if (newWidth > RegexListItemsControlMaxWidth) {
            newWidth = RegexListItemsControlMaxWidth;
        }
        RegexListItemsControl.Width = newWidth;
    }
}
