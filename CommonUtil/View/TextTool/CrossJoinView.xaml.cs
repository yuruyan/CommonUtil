namespace CommonUtil.View;

public partial class CrossJoinView : ResponsivePage {
    public class SimpleText : DependencyObject {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SimpleText), new PropertyMetadata(string.Empty));

        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(CrossJoinView), new PropertyMetadata(""));
    public static readonly DependencyProperty DataListProperty = DependencyProperty.Register("DataList", typeof(ObservableCollection<SimpleText>), typeof(CrossJoinView), new PropertyMetadata());

    /// <summary>
    /// 数据列表
    /// </summary>
    public ObservableCollection<SimpleText> DataList {
        get { return (ObservableCollection<SimpleText>)GetValue(DataListProperty); }
        set { SetValue(DataListProperty, value); }
    }
    /// <summary>
    /// 输出文本
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }

    public CrossJoinView() : base(ResponsiveMode.Variable) {
        DataList = new() { new() };
        InitializeComponent();
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        for (int i = 0; i < DataList.Count; i++) {
            DataList[i].Text = string.Empty;
        }
        OutputText = string.Empty;
    }

    /// <summary>
    /// 处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CrossJoinClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var result = CrossJoin.Join(DataList.Select(
            item => item.Text.ReplaceLineFeedWithLinuxStyle().Split('\n', StringSplitOptions.RemoveEmptyEntries)
        ));
        OutputText = string.Join("\n", result);
    }

    /// <summary>
    /// 添加数据源
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddDataItemClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        DataList.Add(new());
    }

    /// <summary>
    /// DeleteItem
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteListBoxItemClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        DataList.RemoveList(DataListBox.SelectedItems.Cast<SimpleText>().ToList());
    }
}
