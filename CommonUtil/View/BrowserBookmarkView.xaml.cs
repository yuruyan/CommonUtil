namespace CommonUtil.View;

public partial class BrowserBookmarkView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty EdgeBookmarkFilePathProperty = DependencyProperty.Register("EdgeBookmarkFilePath", typeof(string), typeof(BrowserBookmarkView), new PropertyMetadata(""));
    private readonly SaveFileDialog ExportFileDialog = new() {
        Filter = "Excel|*.xlsx",
    };

    public string EdgeBookmarkFilePath {
        get { return (string)GetValue(EdgeBookmarkFilePathProperty); }
        set { SetValue(EdgeBookmarkFilePathProperty, value); }
    }

    public BrowserBookmarkView() {
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
        if (ExportFileDialog.ShowDialog() != true) {
            return;
        }
        var sourcePath = EdgeBookmarkFilePath;
        Task.Run(() => {
            try {
                new EdgeBookmark().ExportBookmarks(
                    sourcePath,
                    ExportFileDialog.FileName
                );
                ExportFileDialog.FileName.NotificationOpenFileInExplorerAsync("导出成功！");
            } catch (Exception error) {
                MessageBoxUtils.Error("导出失败！" + error.Message);
                Logger.Error(error);
            }
        });
    }
}
