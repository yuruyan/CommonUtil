using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CommonUtil.View;

public partial class TempFileVersionControlView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public class WatchFile : DependencyObject {
        public static readonly DependencyProperty FilenameProperty = DependencyProperty.Register("Filename", typeof(string), typeof(WatchFile), new PropertyMetadata(""));
        public static readonly DependencyProperty SaveFolderProperty = DependencyProperty.Register("SaveFolder", typeof(string), typeof(WatchFile), new PropertyMetadata(""));
        public static readonly DependencyProperty StartedProperty = DependencyProperty.Register("Started", typeof(bool), typeof(WatchFile), new PropertyMetadata(false));
        public static readonly DependencyProperty GeneratedFilenamesProperty = DependencyProperty.Register("GeneratedFilenames", typeof(ObservableCollection<string>), typeof(WatchFile), new PropertyMetadata());

        public string Filename {
            get { return (string)GetValue(FilenameProperty); }
            set { SetValue(FilenameProperty, value); }
        }
        public string SaveFolder {
            get { return (string)GetValue(SaveFolderProperty); }
            set { SetValue(SaveFolderProperty, value); }
        }
        public bool Started {
            get { return (bool)GetValue(StartedProperty); }
            set { SetValue(StartedProperty, value); }
        }
        /// <summary>
        /// 生成的文件
        /// </summary>
        public ObservableCollection<string> GeneratedFilenames {
            get { return (ObservableCollection<string>)GetValue(GeneratedFilenamesProperty); }
            set { SetValue(GeneratedFilenamesProperty, value); }
        }
        public TempFileVersionControl TempFileVersionControl { get; set; }
        // 查找时用于标识
        public int Id { get; }

        public WatchFile(int id, TempFileVersionControl tempFileVersionControl) {
            Id = id;
            TempFileVersionControl = tempFileVersionControl;
        }
    }

    public static readonly DependencyProperty WatchFilesProperty = DependencyProperty.Register("WatchFiles", typeof(ObservableCollection<WatchFile>), typeof(TempFileVersionControlView), new PropertyMetadata());
    public static readonly DependencyProperty SelectedWatchFileProperty = DependencyProperty.Register("SelectedWatchFile", typeof(WatchFile), typeof(TempFileVersionControlView), new PropertyMetadata());

    public ObservableCollection<WatchFile> WatchFiles {
        get { return (ObservableCollection<WatchFile>)GetValue(WatchFilesProperty); }
        set { SetValue(WatchFilesProperty, value); }
    }
    public WatchFile? SelectedWatchFile {
        get { return (WatchFile)GetValue(SelectedWatchFileProperty); }
        set { SetValue(SelectedWatchFileProperty, value); }
    }
    private int fileId = 0;
    private readonly OpenFileDialog SelectFileDialog = new() {
        Title = "选择文件",
        Filter = "All Files|*.*"
    };
    private readonly VistaFolderBrowserDialog SaveFolderDialog = new() {
        Description = "选择保存文件夹",
        UseDescriptionForTitle = true
    };
    public TempFileVersionControlView() {
        WatchFiles = new();
        InitializeComponent();
    }

    /// <summary>
    /// 点击修改保存文件目录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveFolderMouseUpHandler(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        if (SelectedWatchFile is null) {
            return;
        }
        SaveFolderDialog.SelectedPath = SelectedWatchFile.SaveFolder;
        if (SaveFolderDialog.ShowDialog(Application.Current.MainWindow) != true) {
            return;
        }
        SelectedWatchFile.SaveFolder = SaveFolderDialog.SelectedPath;
    }

    /// <summary>
    /// 切换监听文件状态
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToggleWatchFileStateClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (SelectedWatchFile is not null) {
            SelectedWatchFile.Started = !SelectedWatchFile.Started;
        }
    }

    /// <summary>
    /// 删除监听文件，简单移除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteWatchFileClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (SelectedWatchFile is not null) {
            SelectedWatchFile.Started = false;
            // 停止监听
            SelectedWatchFile.TempFileVersionControl.Dispose();
            WatchFiles.Remove(SelectedWatchFile);
            SelectedWatchFile = null;
        }
    }

    /// <summary>
    /// 添加监听文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddWatchFileClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (SelectFileDialog.ShowDialog() == false) {
            return;
        }
        var watchFilename = SelectFileDialog.FileName;
        var watchFileInfo = new FileInfo(watchFilename);
        // 默认保存目录，即文件名作为目录
        var saveDirectory = Directory.CreateDirectory(Path.Combine(
            watchFileInfo.DirectoryName!,
            // 去掉后缀名
            watchFileInfo.Name[0..(watchFileInfo.Name.LastIndexOf(watchFileInfo.Extension))]
        )).FullName;
        var tempFileId = fileId++;
        // 创建监听实例
        var fileWatcher = CreateFileWatch(watchFilename, tempFileId);
        // 添加到 WatchFiles
        var newWatchFile = new WatchFile(tempFileId, fileWatcher) {
            Filename = watchFilename,
            SaveFolder = saveDirectory,
            Started = true,
            GeneratedFilenames = new()
        };
        WatchFiles.Add(newWatchFile);
        SelectedWatchFile = newWatchFile;
    }

    /// <summary>
    /// 创建监听文件
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="fileId"></param>
    /// <returns></returns>
    private TempFileVersionControl CreateFileWatch(string filename, int fileId) {
        return TempFileVersionControl.Watch(filename, fileInfo => {
            Dispatcher.Invoke(async () => {
                // 查找
                var targetFile = WatchFiles.FirstOrDefault(f => f.Id == fileId);
                if (targetFile == null) {
                    Logger.Error("TargetFile not found");
                    return;
                }
                // 未开启监听
                if (!targetFile.Started) {
                    return;
                }
                string destFile = Path.Combine(targetFile.SaveFolder, $"{CommonUtils.CuruentSeconds}-{fileInfo.Name}");
                // 创建文件夹
                if (!TaskUtils.Try(() => { Directory.CreateDirectory(targetFile.SaveFolder); return true; })) {
                    var dirName = Path.GetFileName(targetFile.SaveFolder);
                    CommonUITools.Widget.MessageBox.Error($"创建文件夹 {dirName} 失败");
                    Logger.Error($"创建文件夹 {dirName} 失败");
                    return;
                }
                // 保存当前文件副本
                await Task.Run(() => {
                    File.Copy(fileInfo.FullName, destFile, true);
                });
                targetFile.GeneratedFilenames.Add(destFile);
                Logger.Debug($"Temp file {destFile} created");
            });
        });
    }

    /// <summary>
    /// 文件选择改变
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void WatchFilesSelectionChangedHandler(object sender, SelectionChangedEventArgs e) {
        e.Handled = true;
        if (sender is ListBox control && control.SelectedItem is WatchFile file) {
            SelectedWatchFile = file;
        }
    }
}
