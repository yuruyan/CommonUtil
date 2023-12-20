using CommonUtil.Data.Resources;

namespace CommonUtil.View;

public partial class CommonRegexListDialog : BaseDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty RegexListProperty = DependencyProperty.Register("RegexList", typeof(IList<KeyValuePair<string, string>>), typeof(CommonRegexListDialog), new PropertyMetadata());

    /// <summary>
    /// 正则列表
    /// </summary>
    public IList<KeyValuePair<string, string>> RegexList {
        get { return (IList<KeyValuePair<string, string>>)GetValue(RegexListProperty); }
        set { SetValue(RegexListProperty, value); }
    }

    public CommonRegexListDialog() {
        RegexList = new List<KeyValuePair<string, string>>(DataResourceHelper.CommonRegexList);
        InitializeComponent();
        this.EnableAutoResize(
            (double)Resources["RegexListItemsControlMinWidth"],
            (double)Resources["RegexListItemsControlMaxWidth"]
        );
    }
}
