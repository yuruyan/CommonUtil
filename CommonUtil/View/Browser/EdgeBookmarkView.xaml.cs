namespace CommonUtil.View;

public partial class EdgeBookmarkView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public string EdgeBookmarkFilePath {
        get { return (string)GetValue(EdgeBookmarkFilePathProperty); }
        set { SetValue(EdgeBookmarkFilePathProperty, value); }
    }
    public static readonly DependencyProperty EdgeBookmarkFilePathProperty = DependencyProperty.Register("EdgeBookmarkFilePath", typeof(string), typeof(EdgeBookmarkView), new PropertyMetadata(""));

    public EdgeBookmarkView() {
        InitializeComponent();
        string edgeBookmarkFilePath = $@"C:\Users\{Environment.GetEnvironmentVariable("UserName")}\AppData\Local\Microsoft\Edge\User Data\Default\Bookmarks";
        if (File.Exists(edgeBookmarkFilePath)) {
            EdgeBookmarkFilePath = edgeBookmarkFilePath;
        }
    }

    /// <summary>
    /// 导出浏览器书签
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ExportBookmarkClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var dialog = new SaveFileDialog {
            Filter = "Excel|*.xlsx",
        };
        if (dialog.ShowDialog() != true) {
            return;
        }
        Task.Factory.StartNew(() => {
            try {
                new EdgeBookmark().ExportBookmarks(Dispatcher.Invoke(() => EdgeBookmarkFilePath), dialog.FileName);
                NotificationBox.Success("导出成功！", "点击打开", () => {
                    UIUtils.OpenFileInExplorerAsync(dialog.FileName);
                });
            } catch (Exception error) {
                MessageBox.Error("导出失败！" + error.Message);
                Logger.Error(error);
            }
        });
    }
}
