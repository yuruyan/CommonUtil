using System.Collections.Specialized;

namespace CommonUtil.View;

public partial class KeywordFinderView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty SearchDirectoryProperty = DependencyProperty.Register("SearchDirectory", typeof(string), typeof(KeywordFinderView), new PropertyMetadata(""));
    public static readonly DependencyProperty ExcludeDirectoryProperty = DependencyProperty.Register("ExcludeDirectory", typeof(string), typeof(KeywordFinderView), new PropertyMetadata(""));
    public static readonly DependencyProperty ExcludeFileProperty = DependencyProperty.Register("ExcludeFile", typeof(string), typeof(KeywordFinderView), new PropertyMetadata(""));
    public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText", typeof(string), typeof(KeywordFinderView), new PropertyMetadata(""));
    public static readonly DependencyProperty KeywordResultsProperty = DependencyProperty.Register("KeywordResults", typeof(ObservableCollection<KeywordResult>), typeof(KeywordFinderView), new PropertyMetadata());
    public static readonly DependencyProperty IsExcludeDirectorySelectedProperty = DependencyProperty.Register("IsExcludeDirectorySelected", typeof(bool), typeof(KeywordFinderView), new PropertyMetadata(true));
    public static readonly DependencyProperty IsExcludeFileSelectedProperty = DependencyProperty.Register("IsExcludeFileSelected", typeof(bool), typeof(KeywordFinderView), new PropertyMetadata(false));
    public static readonly DependencyProperty IsSearchingFinishedProperty = DependencyProperty.Register("IsSearchingFinished", typeof(bool), typeof(KeywordFinderView), new PropertyMetadata(true));

    /// <summary>
    /// 搜索目录
    /// </summary>
    public string SearchDirectory {
        get { return (string)GetValue(SearchDirectoryProperty); }
        set { SetValue(SearchDirectoryProperty, value); }
    }
    /// <summary>
    /// 排除目录
    /// </summary>
    public string ExcludeDirectory {
        get { return (string)GetValue(ExcludeDirectoryProperty); }
        set { SetValue(ExcludeDirectoryProperty, value); }
    }
    /// <summary>
    /// 排除文件
    /// </summary>
    public string ExcludeFile {
        get { return (string)GetValue(ExcludeFileProperty); }
        set { SetValue(ExcludeFileProperty, value); }
    }
    /// <summary>
    /// 关键字
    /// </summary>
    public string SearchText {
        get { return (string)GetValue(SearchTextProperty); }
        set { SetValue(SearchTextProperty, value); }
    }
    /// <summary>
    /// 排除目录是否勾选
    /// </summary>
    public bool IsExcludeDirectorySelected {
        get { return (bool)GetValue(IsExcludeDirectorySelectedProperty); }
        set { SetValue(IsExcludeDirectorySelectedProperty, value); }
    }
    /// <summary>
    /// 排除文件是否勾选
    /// </summary>
    public bool IsExcludeFileSelected {
        get { return (bool)GetValue(IsExcludeFileSelectedProperty); }
        set { SetValue(IsExcludeFileSelectedProperty, value); }
    }
    /// <summary>
    /// 查询结果
    /// </summary>
    public ObservableCollection<KeywordResult> KeywordResults {
        get { return (ObservableCollection<KeywordResult>)GetValue(KeywordResultsProperty); }
        set { SetValue(KeywordResultsProperty, value); }
    }
    /// <summary>
    /// 搜索是否结束
    /// </summary>
    public bool IsSearchingFinished {
        get { return (bool)GetValue(IsSearchingFinishedProperty); }
        set { SetValue(IsSearchingFinishedProperty, value); }
    }
    private KeywordFinder? KeywordFinder;
    /// <summary>
    /// 上次查询目录
    /// </summary>
    private string LastSearchDirectory = string.Empty;
    /// <summary>
    /// 当前 Window
    /// </summary>
    private Window CurrentWindow = Application.Current.MainWindow;

    public KeywordFinderView() {
        KeywordResults = new();
        KeywordResults.CollectionChanged += KeywordResultsCollectionChanged;
        InitializeComponent();
        this.SetLoadedOnceEventHandler(static (sender, _) => {
            if (sender is KeywordFinderView self) {
                self.CurrentWindow = Window.GetWindow(self);
            }
        });
    }

    protected override void IsExpandedPropertyChangedHandler(ResponsivePage self, DependencyPropertyChangedEventArgs e) {
        if (IsExpanded) {
            SecondRowDefinition.Height = new(0);
            SecondColumnDefinition.Width = new(1, GridUnitType.Star);
            Grid.SetColumn(ResultPanel, 1);
            Grid.SetRow(ResultPanel, 0);
        } else {
            SecondRowDefinition.Height = new(1, GridUnitType.Star);
            SecondColumnDefinition.Width = new(0);
            Grid.SetColumn(ResultPanel, 0);
            Grid.SetRow(ResultPanel, 1);
        }
    }

    private void KeywordResultsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            // 获取相对路径
            if (e.NewItems is null) {
                return;
            }
            foreach (var item in e.NewItems) {
                if (item is KeywordResult result) {
                    result.Filename = result.Filename.Replace('/', '\\')[(SearchDirectory.Replace('/', '\\').Length + 1)..];
                }
            }
        }
    }

    /// <summary>
    /// 处理查询
    /// </summary>
    private void HandleKeywordFinding() {
        if (string.IsNullOrEmpty(SearchDirectory.Trim())) {
            MessageBoxUtils.Info("请选择查询目录！");
            return;
        }
        if (string.IsNullOrEmpty(SearchText)) {
            MessageBoxUtils.Info("搜索文本不能为空！");
            return;
        }
        if (!IsSearchingFinished) {
            MessageBoxUtils.Info("正在搜索...");
            return;
        }
        ResultNumber.Visibility = Visibility.Visible;
        IsSearchingFinished = false;
        var excludeDirs = new List<string>();
        var excludeFiles = new List<string>();
        if (IsExcludeDirectorySelected) {
            excludeDirs.AddRange(
                ExcludeDirectory.ReplaceLineFeedWithLinuxStyle()
                .Split('\n')
                .Select(s => s.Trim())
                .Where(s => s.Any())
            );
        }
        if (IsExcludeFileSelected) {
            excludeFiles.AddRange(
                ExcludeFile.ReplaceLineFeedWithLinuxStyle()
                .Split('\n')
                .Select(s => s.Trim())
                .Where(s => s.Any())
            );
        }
        // 检查搜索目录是否改变
        if (LastSearchDirectory != SearchDirectory) {
            LastSearchDirectory = SearchDirectory;
            Task.Run(() => {
                try {
                    var keywordFinder = new KeywordFinder(LastSearchDirectory, excludeDirs, excludeFiles);
                    Dispatcher.Invoke(() => KeywordFinder = keywordFinder);
                    FindKeyword(excludeDirs, excludeFiles);
                } catch (Exception error) {
                    MessageBoxUtils.Error(error.Message);
                    Logger.Error(error);
                }
            });
            return;
        }
        FindKeyword(excludeDirs, excludeFiles);
    }

    /// <summary>
    /// 查询点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FindKeywordClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        HandleKeywordFinding();
    }

    /// <summary>
    /// 查找关键字
    /// </summary>
    /// <param name="excludeDirs"></param>
    /// <param name="excludeFiles"></param>
    private void FindKeyword(List<string> excludeDirs, List<string> excludeFiles) {
        var value = Dispatcher.Invoke(() => new {
            SearchText,
            KeywordResults,
        });
        var searchText = value.SearchText;
        var keywordResults = value.KeywordResults;
        Task.Run(() => {
            try {
                CommonUtils.NullCheck(KeywordFinder)
                .FindKeyword(searchText, excludeDirs, excludeFiles, keywordResults);
            } catch (OperationCanceledException) {
                // 忽略
            } catch (Exception error) {
                MessageBoxUtils.Error(error.Message);
                Logger.Error(error);
            } finally {
                Dispatcher.Invoke(() => IsSearchingFinished = true);
            }
        });
    }

    /// <summary>
    /// 选择搜索目录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectSearchDirectoryClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var dialog = new VistaFolderBrowserDialog {
            Description = "选择搜索目录",
            UseDescriptionForTitle = true
        };
        if (dialog.ShowDialog(Application.Current.MainWindow) == true) {
            SearchDirectory = dialog.SelectedPath;
        }
    }

    /// <summary>
    /// 打开搜索文件夹
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenSearchDirectoryMouseUp(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        if (sender is TextBlock element) {
            element.Text.OpenFileInExplorerAsync();
        }
    }

    /// <summary>
    /// 打开文件点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenFileClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is KeywordResult result) {
            Path.Combine(SearchDirectory, result.Filename).OpenFileWithAsync();
        }
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenDirectoryClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is KeywordResult result) {
            Path.Combine(SearchDirectory, result.Filename).OpenFileInExplorerAsync();
        }
    }

    /// <summary>
    /// 搜索正则框 KeyUp
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SearchTextBoxKeyUpHandler(object sender, KeyEventArgs e) {
        e.Handled = true;
        if (e.Key == Key.Enter) {
            HandleKeywordFinding();
        }
    }

    /// <summary>
    /// 取消搜索
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelFindKeywordClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        KeywordFinder?.CancelFinding();
        IsSearchingFinished = true;
    }
}
