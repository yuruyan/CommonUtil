namespace CommonUtil.View;

public partial class AsciiTableView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty AsciiTableListProperty = DependencyProperty.Register("AsciiTableList", typeof(ExtendedObservableCollection<AsciiInfo>), typeof(AsciiTableView), new PropertyMetadata());

    /// <summary>
    /// ASCII 列表
    /// </summary>
    public ExtendedObservableCollection<AsciiInfo> AsciiTableList {
        get { return (ExtendedObservableCollection<AsciiInfo>)GetValue(AsciiTableListProperty); }
        set { SetValue(AsciiTableListProperty, value); }
    }

    public AsciiTableView() {
        AsciiTableList = new();
        InitializeComponent();
        this.SetLoadedOnceEventHandler(static (sender, _) => {
            if (sender is not AsciiTableView self) {
                return;
            }
            self.AsciiTableList.AddRange(AsciiTable.GetAsciiInfoList());
        });
    }

    /// <summary>
    /// 复制详情
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyDetailClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var sb = new StringBuilder();
        foreach (var item in AsciiListView.SelectedItems) {
            var info = (AsciiInfo)item;
            sb.Append(info.Binary).Append('\t');
            sb.Append(info.Octal).Append('\t');
            sb.Append(info.Decimal).Append('\t');
            sb.Append(info.HexaDecimal).Append('\t');
            sb.Append(info.Character).Append('\t');
            sb.Append(info.HtmlEntity).Append('\t');
            sb.Append(info.Description).Append('\n');
        }
        Clipboard.SetDataObject(sb.ToString());
        MessageBoxUtils.Success("已复制");
    }
}
