using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace CommonUtil.View;

public partial class FileSplitView : Page {
    /// <summary>
    /// 文件大小类型
    /// </summary>
    public enum FileSizeType {
        Kb,
        Mb,
        Gb
    }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 更新进度间隔时间
    /// </summary>
    private const int UpdateWorkingProcessInterval = 250;
    /// <summary>
    /// 文件大小类型选项
    /// </summary>
    private static readonly IDictionary<FileSizeType, uint> FileSizeTypeOptionMap = new Dictionary<FileSizeType, uint>() {
        {FileSizeType.Kb, 1024 },
        {FileSizeType.Mb, 1024 * 1024 },
        {FileSizeType.Gb, 1024 * 1024 * 1024 },
    };

    public static readonly DependencyProperty SplitFilePathProperty = DependencyProperty.Register("SplitFilePath", typeof(string), typeof(FileSplitView), new PropertyMetadata(""));
    public static readonly DependencyProperty SplitBySizeProperty = DependencyProperty.Register("SplitBySize", typeof(double), typeof(FileSplitView), new PropertyMetadata(1.0));
    public static readonly DependencyProperty SplitFileSaveDirectoryProperty = DependencyProperty.Register("SplitFileSaveDirectory", typeof(string), typeof(FileSplitView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileSizeTypeOptionsProperty = DependencyProperty.Register("FileSizeTypeOptions", typeof(IList<FileSizeType>), typeof(FileSplitView), new PropertyMetadata());
    public static readonly DependencyProperty WorkingProcessProperty = DependencyProperty.Register("WorkingProcess", typeof(double), typeof(FileSplitView), new PropertyMetadata(0.0));
    public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register("IsWorking", typeof(bool), typeof(FileSplitView), new PropertyMetadata(false));
    public static readonly DependencyProperty SplitByCountProperty = DependencyProperty.Register("SplitByCount", typeof(double), typeof(FileSplitView), new PropertyMetadata(1.0));
    public static readonly DependencyProperty SplitBySizeComboBoxSelectedIndexProperty = DependencyProperty.Register("SplitBySizeComboBoxSelectedIndex", typeof(int), typeof(FileSplitView), new PropertyMetadata(1));
    public static readonly DependencyProperty SplitFileSizeProperty = DependencyProperty.Register("SplitFileSize", typeof(ulong), typeof(FileSplitView), new PropertyMetadata(0UL));

    /// <summary>
    /// 按文件数量分割个数
    /// </summary>
    public double SplitByCount {
        get { return (double)GetValue(SplitByCountProperty); }
        set { SetValue(SplitByCountProperty, value); }
    }
    /// <summary>
    /// 按文件大小分割的大小
    /// </summary>
    public double SplitBySize {
        get { return (double)GetValue(SplitBySizeProperty); }
        set { SetValue(SplitBySizeProperty, value); }
    }
    /// <summary>
    /// 要分割的文件路径
    /// </summary>
    public string SplitFilePath {
        get { return (string)GetValue(SplitFilePathProperty); }
        set { SetValue(SplitFilePathProperty, value); }
    }
    /// <summary>
    /// 要分割的文件大小
    /// </summary>
    public ulong SplitFileSize {
        get { return (ulong)GetValue(SplitFileSizeProperty); }
        set { SetValue(SplitFileSizeProperty, value); }
    }
    /// <summary>
    /// 分割文件保存文件夹
    /// </summary>
    public string SplitFileSaveDirectory {
        get { return (string)GetValue(SplitFileSaveDirectoryProperty); }
        set { SetValue(SplitFileSaveDirectoryProperty, value); }
    }
    /// <summary>
    /// 文件大小类型
    /// </summary>
    public IList<FileSizeType> FileSizeTypeOptions {
        get { return (IList<FileSizeType>)GetValue(FileSizeTypeOptionsProperty); }
        set { SetValue(FileSizeTypeOptionsProperty, value); }
    }
    /// <summary>
    /// 按大小分割文件大小类型选项下标
    /// </summary>
    public int SplitBySizeComboBoxSelectedIndex {
        get { return (int)GetValue(SplitBySizeComboBoxSelectedIndexProperty); }
        set { SetValue(SplitBySizeComboBoxSelectedIndexProperty, value); }
    }
    /// <summary>
    /// 分割文件进度
    /// </summary>
    public double WorkingProcess {
        get { return (double)GetValue(WorkingProcessProperty); }
        set { SetValue(WorkingProcessProperty, value); }
    }
    /// <summary>
    /// 是否正在分割文件
    /// </summary>
    public bool IsWorking {
        get { return (bool)GetValue(IsWorkingProperty); }
        set { SetValue(IsWorkingProperty, value); }
    }
    /// <summary>
    /// 上次分割文件更新时间
    /// </summary>
    private DateTime LastUpdateProcessTime = DateTime.Now;
    /// <summary>
    /// 正在分割的文件路径
    /// </summary>
    private string WorkingSplitFilePath = string.Empty;
    /// <summary>
    /// 是否请求取消
    /// </summary>
    private bool IsCancelRequested = false;

    public FileSplitView() {
        DependencyPropertyDescriptor.FromProperty(SplitFilePathProperty, typeof(FileSplitView))
            .AddValueChanged(this, (o, e) => SplitFileSize = (ulong)new FileInfo(SplitFilePath).Length);
        FileSizeTypeOptions = FileSizeTypeOptionMap.Keys.ToArray();
        InitializeComponent();
    }

    /// <summary>
    /// 选择文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectFileClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var openFileDialog = new OpenFileDialog() {
            Title = "选择文件",
            Filter = "All Files|*.*"
        };
        if (openFileDialog.ShowDialog() != true) {
            return;
        }
        SplitFilePath = openFileDialog.FileName;
    }

    /// <summary>
    /// 选择分割文件保存目录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectSaveFolderClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        var dialog = new VistaFolderBrowserDialog {
            Description = "选择保存目录",
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog(Application.Current.MainWindow) == true) {
            SplitFileSaveDirectory = dialog.SelectedPath;
        }
    }

    /// <summary>
    /// 获取按文件数量方式分割文件的大小
    /// </summary>
    /// <returns></returns>
    private ulong GetPerFileSizeByFileCount() {
        return (ulong)(SplitFileSize / SplitByCount);
    }

    /// <summary>
    /// 获取按文件大小方式分割文件的大小
    /// </summary>
    /// <returns></returns>
    private ulong GetPerFileSizeByFileSize() {
        return (ulong)(FileSizeTypeOptionMap[FileSizeTypeOptions[SplitBySizeComboBoxSelectedIndex]] * SplitBySize);
    }

    /// <summary>
    /// 分割文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void StartClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (IsWorking) {
            return;
        }
        // 检查输入合法性
        if (!CheckSplitFileInputValidation()) {
            return;
        }

        IsWorking = true;
        WorkingProcess = 0;
        WorkingSplitFilePath = SplitFilePath;
        ulong perSize = SplitChoiceComboBox.SelectedIndex == 0
            ? GetPerFileSizeByFileSize() : GetPerFileSizeByFileCount();
        string filepath = SplitFilePath;
        string saveDir = SplitFileSaveDirectory;
        // 开始分割
        try {
            await Task.Run(() => FileMergeSplit.SplitFile(
                filepath,
                saveDir,
                perSize,
                process => {
                    if ((DateTime.Now - LastUpdateProcessTime).TotalMilliseconds > UpdateWorkingProcessInterval) {
                        LastUpdateProcessTime = DateTime.Now;
                        Dispatcher.Invoke(() => WorkingProcess = process);
                    }
                })
            );
            // 没有取消则提示
            if (!IsCancelRequested) {
                MessageBox.Success("分割完成");
            }
        } catch (Exception error) {
            MessageBox.Error($"分割失败：{error.Message}");
        }
        IsCancelRequested = IsWorking = false;
    }

    /// <summary>
    /// 检查分割文件输入有效性
    /// </summary>
    /// <returns></returns>
    private bool CheckSplitFileInputValidation() {
        if (!UIUtils.CheckInputNullOrEmpty(new KeyValuePair<string?, string>[] {
            new (SplitFilePath, "要分割的文件不能为空"),
            new (SplitFileSaveDirectory, "保存目录不能为空"),
        })) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 取消任务
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelClickHandler(object sender, RoutedEventArgs e) {
        IsCancelRequested = true;
        FileMergeSplit.CancelSplitFile(WorkingSplitFilePath);
    }
}
