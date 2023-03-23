namespace CommonUtil.View;

public partial class CollectionToolView : ResponsivePage {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty InputText1Property = DependencyProperty.Register("InputText1", typeof(string), typeof(CollectionToolView), new PropertyMetadata(""));
    public static readonly DependencyProperty InputText2Property = DependencyProperty.Register("InputText2", typeof(string), typeof(CollectionToolView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(CollectionToolView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileName1Property = DependencyProperty.Register("FileName1", typeof(string), typeof(CollectionToolView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFile1Property = DependencyProperty.Register("HasFile1", typeof(bool), typeof(CollectionToolView), new PropertyMetadata(false));
    public static readonly DependencyProperty FileName2Property = DependencyProperty.Register("FileName2", typeof(string), typeof(CollectionToolView), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty HasFile2Property = DependencyProperty.Register("HasFile2", typeof(bool), typeof(CollectionToolView), new PropertyMetadata(false));

    /// <summary>
    /// 输入
    /// </summary>
    public string InputText2 {
        get { return (string)GetValue(InputText2Property); }
        set { SetValue(InputText2Property, value); }
    }
    /// <summary>
    /// 输入
    /// </summary>
    public string InputText1 {
        get { return (string)GetValue(InputText1Property); }
        set { SetValue(InputText1Property, value); }
    }
    /// <summary>
    /// 输出结果
    /// </summary>
    public string OutputText {
        get { return (string)GetValue(OutputTextProperty); }
        set { SetValue(OutputTextProperty, value); }
    }
    /// <summary>
    /// 是否有文件1
    /// </summary>
    public bool HasFile1 {
        get { return (bool)GetValue(HasFile1Property); }
        set { SetValue(HasFile1Property, value); }
    }
    /// <summary>
    /// 文件名1
    /// </summary>
    public string FileName1 {
        get { return (string)GetValue(FileName1Property); }
        set { SetValue(FileName1Property, value); }
    }
    /// <summary>
    /// 是否有文件2
    /// </summary>
    public bool HasFile2 {
        get { return (bool)GetValue(HasFile2Property); }
        set { SetValue(HasFile2Property, value); }
    }
    /// <summary>
    /// 文件名2
    /// </summary>
    public string FileName2 {
        get { return (string)GetValue(FileName2Property); }
        set { SetValue(FileName2Property, value); }
    }

    private readonly SaveFileDialog SaveFileDialog = new() {
        Title = "保存文件",
        Filter = "文本文件|*.txt|All Files|*.*"
    };
    public delegate IList<string> ProcessHanlder(IEnumerable<string> list1, IEnumerable<string> list2);

    public CollectionToolView() {
        InitializeComponent();
    }

    /// <summary>
    /// 复制结果
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyResultClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        Clipboard.SetDataObject(OutputText);
        MessageBoxUtils.Success("已复制");
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearInputClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        InputText2 = InputText1 = OutputText = string.Empty;
        DragDropTextBox1.Clear();
        DragDropTextBox2.Clear();
    }

    /// <summary>
    /// 文本处理
    /// </summary>
    /// <param name="func"></param>
    private void StringProcess(ProcessHanlder func) {
        OutputText = string.Join(
                        '\n',
                        func(
                            InputText1.ReplaceLineFeedWithLinuxStyle().Split('\n'),
                            InputText2.ReplaceLineFeedWithLinuxStyle().Split('\n')
                        )
                     );
    }

    /// <summary>
    /// 文件文本处理
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    private async Task FileProcess(ProcessHanlder func) {
        if (SaveFileDialog.ShowDialog() != true) {
            return;
        }
        var savePath = SaveFileDialog.FileName;
        // 解码
        var list1 = InputText1.ReplaceLineFeedWithLinuxStyle().Split('\n');
        var list2 = InputText2.ReplaceLineFeedWithLinuxStyle().Split('\n');
        // 读取文件
        try {
            if (HasFile1) {
                list1 = File.ReadAllLines(FileName1);
            }
            if (HasFile2) {
                list2 = File.ReadAllLines(FileName2);
            }
        } catch {
            MessageBoxUtils.Error("文件读取失败");
            return;
        }

        // 处理
        await UIUtils.CreateFileProcessTask(
            () => {
                var result = func(list1, list2);
                // 保存文件
                File.WriteAllLines(savePath, result);
            },
            savePath
        );
    }

    /// <summary>
    /// 检查是否可能是二进制文件
    /// </summary>
    /// <returns>点击继续返回 true</returns>
    private async Task<bool> CheckBinaryFile() {
        // 二进制文件警告
        if (HasFile1 && CommonUtils.IsLikelyBinaryFile(FileName1) || HasFile2 && CommonUtils.IsLikelyBinaryFile(FileName2)) {
            WarningDialog dialog = WarningDialog.Shared;
            dialog.DetailText = "文件可能不是二进制文件，是否继续？";
            if (await dialog.ShowAsync() != ModernWpf.Controls.ContentDialogResult.Primary) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 并集
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void UnionClickHandler(object sender, RoutedEventArgs e) {
        // 没有文件
        if (!HasFile1 && !HasFile2) {
            StringProcess(CollectionTool.Union);
            return;
        }
        // 检查二进制文件
        if (!await CheckBinaryFile()) {
            return;
        }
        ThrottleUtils.ThrottleAsync(UnionClickHandler, () => FileProcess(CollectionTool.Union));
    }

    /// <summary>
    /// 交集
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void IntersectClickHandler(object sender, RoutedEventArgs e) {
        // 没有文件
        if (!HasFile1 && !HasFile2) {
            StringProcess(CollectionTool.Intersect);
            return;
        }
        // 检查二进制文件
        if (!await CheckBinaryFile()) {
            return;
        }
        ThrottleUtils.ThrottleAsync(IntersectClickHandler, () => FileProcess(CollectionTool.Intersect));
    }

    /// <summary>
    /// 差集
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ExceptClickHandler(object sender, RoutedEventArgs e) {
        // 没有文件
        if (!HasFile1 && !HasFile2) {
            StringProcess(CollectionTool.Except);
            return;
        }
        // 检查二进制文件
        if (!await CheckBinaryFile()) {
            return;
        }
        ThrottleUtils.ThrottleAsync(ExceptClickHandler, () => FileProcess(CollectionTool.Except));
    }

    /// <summary>
    /// DragDropEvent1
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DragDropEventHandler1(object sender, object e) {
        if (e is IEnumerable<string> array) {
            if (!array.Any()) {
                return;
            }
            // 判断是否为文件
            if (File.Exists(array.First())) {
                FileName1 = array.First();
            } else {
                DragDropTextBox1.Clear();
            }
        }
    }

    /// <summary>
    /// DragDropEvent2
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DragDropEventHandler2(object sender, object e) {
        if (e is IEnumerable<string> array) {
            if (!array.Any()) {
                return;
            }
            // 判断是否为文件
            if (File.Exists(array.First())) {
                FileName2 = array.First();
            } else {
                DragDropTextBox2.Clear();
            }
        }
    }
}
