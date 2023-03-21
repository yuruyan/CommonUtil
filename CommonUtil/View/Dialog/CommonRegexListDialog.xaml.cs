namespace CommonUtil.View;

public partial class CommonRegexListDialog : BaseDialog {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty RegexListProperty = DependencyProperty.Register("RegexList", typeof(IEnumerable<KeyValuePair<string, string>>), typeof(CommonRegexListDialog), new PropertyMetadata());
    private static readonly IEnumerable<KeyValuePair<string, string>> CommonRegexList;

    /// <summary>
    /// 正则列表
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> RegexList {
        get { return (IEnumerable<KeyValuePair<string, string>>)GetValue(RegexListProperty); }
        set { SetValue(RegexListProperty, value); }
    }

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
        this.EnableContentDialogAutoResize(
            (double)Resources["RegexListItemsControlMinWidth"],
            (double)Resources["RegexListItemsControlMaxWidth"]
        );
    }
}
