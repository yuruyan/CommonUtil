using CommonUITools.Utils;
using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommonUtil.View {
    public partial class TempFileVersionControlView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public class WatchFile : DependencyObject {
            public static readonly DependencyProperty FilenameProperty = DependencyProperty.Register("Filename", typeof(string), typeof(WatchFile), new PropertyMetadata(""));
            public static readonly DependencyProperty SaveFolderProperty = DependencyProperty.Register("SaveFolder", typeof(string), typeof(WatchFile), new PropertyMetadata(""));
            public static readonly DependencyProperty StartedProperty = DependencyProperty.Register("Started", typeof(bool), typeof(WatchFile), new PropertyMetadata(false));

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
            var dialog = new VistaFolderBrowserDialog {
                SelectedPath = SelectedWatchFile.SaveFolder,
                Description = "选择保存文件夹",
                UseDescriptionForTitle = true // This applies to the Vista style dialog only, not the old dialog.
            };
            if (dialog.ShowDialog(Application.Current.MainWindow) != true) {
                return;
            }
            SelectedWatchFile.SaveFolder = dialog.SelectedPath;
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
            var openFileDialog = new OpenFileDialog() {
                Title = "选择文件",
                Filter = "All Files|*.*"
            };
            if (openFileDialog.ShowDialog() == false) {
                return;
            }
            var watchFile = openFileDialog.FileName;
            var watchFileInfo = new FileInfo(watchFile);
            // 默认保存目录，即文件名作为目录
            var saveFolderInfo = Directory.CreateDirectory(Path.Combine(
                watchFileInfo.DirectoryName,
                // 去掉后缀名
                watchFileInfo.Name[0..(watchFileInfo.Name.LastIndexOf(watchFileInfo.Extension))]
            ));
            var tempFileId = fileId++;
            // 创建监听实例
            var fileWatcher = TempFileVersionControl.Watch(watchFile, f => {
                // 查找
                WatchFile? targetFile = Dispatcher.Invoke(() => WatchFiles.FirstOrDefault(f => f.Id == tempFileId));
                if (targetFile == null) {
                    Console.WriteLine("targetFile not found");
                    return;
                }
                // 未开启监听
                if (!Dispatcher.Invoke(() => targetFile.Started)) {
                    return;
                }
                string destFile = Path.Combine(Dispatcher.Invoke(() => targetFile.SaveFolder), $"{CommonUtils.CuruentSeconds}-{f.Name}");
                // 保存当前文件副本
                File.Copy(f.FullName, destFile, true);
                Logger.Debug($"copied {destFile}");
            });
            // 添加到 WatchFiles
            var newWatchFile = new WatchFile(tempFileId, fileWatcher) {
                Filename = watchFile,
                SaveFolder = saveFolderInfo.FullName,
                Started = true,
            };
            WatchFiles.Add(newWatchFile);
            SelectedWatchFile = newWatchFile;
        }

        /// <summary>
        /// 文件选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WatchFilesSelectionChangedHandler(object sender, SelectionChangedEventArgs e) {
            if (sender is ListBox control && control.SelectedItem is WatchFile file) {
                SelectedWatchFile = file;
            }
        }
    }
}
