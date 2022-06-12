using CommonUtil.Core;
using CommonUITools.Utils;
using Microsoft.Win32;
using NLog;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace CommonUtil.View {
    public partial class FileMergeSplitView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty IsSizeOptionSelectedProperty = DependencyProperty.Register("IsSizeOptionSelected", typeof(bool), typeof(FileMergeSplitView), new PropertyMetadata(true));
        public static readonly DependencyProperty SplitSizeOptionInputTextProperty = DependencyProperty.Register("SplitSizeOptionInputText", typeof(string), typeof(FileMergeSplitView), new PropertyMetadata("100"));
        public static readonly DependencyProperty SplitNumberOptionInputTextProperty = DependencyProperty.Register("SplitNumberOptionInputText", typeof(string), typeof(FileMergeSplitView), new PropertyMetadata("10"));
        public static readonly DependencyProperty SplitFilePathProperty = DependencyProperty.Register("SplitFilePath", typeof(string), typeof(FileMergeSplitView), new PropertyMetadata(""));
        public static readonly DependencyProperty SplitFileSizeProperty = DependencyProperty.Register("SplitFileSize", typeof(long), typeof(FileMergeSplitView), new PropertyMetadata(0L));
        public static readonly DependencyProperty SplitFileSaveDirectoryProperty = DependencyProperty.Register("SplitFileSaveDirectory", typeof(string), typeof(FileMergeSplitView), new PropertyMetadata(""));
        public static readonly DependencyProperty FileSizeOptionsProperty = DependencyProperty.Register("FileSizeOptions", typeof(List<string>), typeof(FileMergeSplitView), new PropertyMetadata());
        public static readonly DependencyProperty MergeFileDirectoryProperty = DependencyProperty.Register("MergeFileDirectory", typeof(string), typeof(FileMergeSplitView), new PropertyMetadata(""));
        public static readonly DependencyProperty MergeFileSavePathProperty = DependencyProperty.Register("MergeFileSavePath", typeof(string), typeof(FileMergeSplitView), new PropertyMetadata(""));
        public static readonly DependencyProperty MergeFilesProperty = DependencyProperty.Register("MergeFiles", typeof(ObservableCollection<string>), typeof(FileMergeSplitView), new PropertyMetadata());

        /// <summary>
        /// 是否选中按文件大小分割
        /// </summary>
        public bool IsSizeOptionSelected {
            get { return (bool)GetValue(IsSizeOptionSelectedProperty); }
            set { SetValue(IsSizeOptionSelectedProperty, value); }
        }
        /// <summary>
        /// 按文件大小分割输入
        /// </summary>
        public string SplitSizeOptionInputText {
            get { return (string)GetValue(SplitSizeOptionInputTextProperty); }
            set { SetValue(SplitSizeOptionInputTextProperty, value); }
        }
        /// <summary>
        /// 按文件数目分割输入
        /// </summary>
        public string SplitNumberOptionInputText {
            get { return (string)GetValue(SplitNumberOptionInputTextProperty); }
            set { SetValue(SplitNumberOptionInputTextProperty, value); }
        }
        /// <summary>
        /// 分割文件路径
        /// </summary>
        public string SplitFilePath {
            get { return (string)GetValue(SplitFilePathProperty); }
            set { SetValue(SplitFilePathProperty, value); }
        }
        /// <summary>
        /// 分割文件大小
        /// </summary>
        public long SplitFileSize {
            get { return (long)GetValue(SplitFileSizeProperty); }
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
        public List<string> FileSizeOptions {
            get { return (List<string>)GetValue(FileSizeOptionsProperty); }
            set { SetValue(FileSizeOptionsProperty, value); }
        }
        /// <summary>
        /// 合并文件输入
        /// </summary>
        public string MergeFileDirectory {
            get { return (string)GetValue(MergeFileDirectoryProperty); }
            set { SetValue(MergeFileDirectoryProperty, value); }
        }
        /// <summary>
        /// 合并文件保存路径
        /// </summary>
        public string MergeFileSavePath {
            get { return (string)GetValue(MergeFileSavePathProperty); }
            set { SetValue(MergeFileSavePathProperty, value); }
        }
        /// <summary>
        /// 合并文件路径列表
        /// </summary>
        public ObservableCollection<string> MergeFiles {
            get { return (ObservableCollection<string>)GetValue(MergeFilesProperty); }
            set { SetValue(MergeFilesProperty, value); }
        }

        /// <summary>
        /// 分割文件进度监控
        /// </summary>
        private FileMergeSplit.ProcessMonitor SplitFileProcessMonitor = new();
        /// <summary>
        /// 合并文件进度监控
        /// </summary>
        private FileMergeSplit.ProcessMonitor MergeFileProcessMonitor = new();
        /// <summary>
        /// 分割文件进度监控 Timer
        /// </summary>
        private System.Timers.Timer SplitFileProcessTimer;
        /// <summary>
        /// 合并文件进度监控 Timer
        /// </summary>
        private System.Timers.Timer MergeFileProcessTimer;
        /// <summary>
        /// 是否正在分割文件
        /// </summary>
        private bool IsSplitingFile = false;
        /// <summary>
        /// 是否正在合并文件
        /// </summary>
        private bool IsMergingFile = false;

        public FileMergeSplitView() {
            FileSizeOptions = new() {
                "KB",
                "MB",
                "GB",
                "%百分比",
            };
            MergeFiles = new();
            SplitFileProcessTimer = new System.Timers.Timer(250);
            SplitFileProcessTimer.Elapsed += UpdateSplitFileProcess;
            MergeFileProcessTimer = new System.Timers.Timer(250);
            MergeFileProcessTimer.Elapsed += UpdateMergeFileProcess;
            InitializeComponent();
        }

        /// <summary>
        /// 更新 MergeFileProcess
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void UpdateMergeFileProcess(object? sender, ElapsedEventArgs e) {
            Dispatcher.Invoke(() => {
                MergeFileProgressBar.Value = (int)(MergeFileProcessMonitor.Process * 100);
            });
        }

        /// <summary>
        /// 更新 SplitFileProcess
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void UpdateSplitFileProcess(object? sender, ElapsedEventArgs e) {
            Dispatcher.Invoke(() => {
                SplitFileProgressBar.Value = (int)(SplitFileProcessMonitor.Process * 100);
            });
        }

        /// <summary>
        /// 分割选项改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SplitOptionChanged(object sender, SelectionChangedEventArgs e) {
            if (SplitSizeOption.IsChecked == true) {
                IsSizeOptionSelected = true;
            } else if (SplitNumberOption.IsChecked == true) {
                IsSizeOptionSelected = false;
            }
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
            try {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                SplitFilePath = fileInfo.FullName;
                SplitFileSize = fileInfo.Length;
            } catch (Exception error) {
                Logger.Info(error);
                CommonUITools.Widget.MessageBox.Error("文件不存在！");
            }
        }

        /// <summary>
        /// 选择分割文件保存目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFileSplitSaveDirClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "选择保存文件夹";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.

            if (dialog.ShowDialog(Application.Current.MainWindow) == true) {
                SplitFileSaveDirectory = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 分割文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SplitFileClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            if (IsSplitingFile) {
                CommonUITools.Widget.MessageBox.Info("正在分割文件");
                return;
            }
            if (!CheckSplitFileInputValidation()) {
                return;
            }
            long perSize = 0;
            if (IsSizeOptionSelected) {
                double size = 0;
                try {
                    size = Convert.ToDouble(SplitSizeOptionInputText);
                    if (size <= 0) {
                        throw new OverflowException("输入无效！");
                    }
                } catch (Exception error) {
                    Logger.Error(error);
                    CommonUITools.Widget.MessageBox.Error("输入无效！");
                    return;
                }
                string? value = FileSizeOptionComboBox.SelectedValue.ToString();
                if (value == null) {
                    Logger.Error("选择错误");
                    CommonUITools.Widget.MessageBox.Error("选择错误！");
                    return;
                }
                if (value == "KB") {
                    perSize = (long)(size * 0x400);
                } else if (value == "MB") {
                    perSize = (long)(size * 0x100000);
                } else if (value == "GB") {
                    perSize = (long)(size * 0x40000000);
                } else if (value.Contains('%')) {
                    perSize = (long)(SplitFileSize * (size / 100));
                }
            } else {
                try {
                    perSize = SplitFileSize / Convert.ToUInt16(SplitNumberOptionInputText);
                } catch (Exception error) {
                    Logger.Error(error);
                    CommonUITools.Widget.MessageBox.Error("输入无效！");
                    return;
                }
            }
            if (perSize <= 1) {
                CommonUITools.Widget.MessageBox.Error("输入无效！");
                return;
            }

            string filepath = SplitFilePath;
            string saveDir = SplitFileSaveDirectory;
            Task.Run(() => {
                try {
                    IsSplitingFile = true;
                    SplitFileProcessTimer.Start();
                    FileMergeSplit.SplitFile(filepath, saveDir, perSize, SplitFileProcessMonitor);
                    // 等待一段时间后再停止更新
                    ThreadPool.QueueUserWorkItem(p => {
                        Thread.Sleep((int)(SplitFileProcessTimer.Interval * 2));
                        SplitFileProcessTimer.Stop();
                    });
                } catch (Exception error) {
                    Logger.Error(error);
                    CommonUITools.Widget.MessageBox.Error("分割失败！");
                } finally {
                    IsSplitingFile = false;
                }
                Dispatcher.Invoke(() => {
                    CommonUITools.Widget.MessageBox.Success("分割完成！");
                });
            });
        }

        /// <summary>
        /// 检查分割文件输入有效性
        /// </summary>
        /// <returns></returns>
        private bool CheckSplitFileInputValidation() {
            if (string.IsNullOrEmpty(SplitFilePath)) {
                CommonUITools.Widget.MessageBox.Info("请选择要分割的文件！");
                return false;
            }
            if (string.IsNullOrEmpty(SplitFileSaveDirectory)) {
                CommonUITools.Widget.MessageBox.Info("请选择保存目录！");
                return false;
            }
            string inputNumber = string.Empty;
            if (IsSizeOptionSelected) {
                inputNumber = SplitSizeOptionInputText;
            } else {
                inputNumber = SplitNumberOptionInputText;
            }
            // 检查输入
            try {
                Convert.ToDouble(inputNumber);
            } catch (Exception e) {
                Logger.Info(e);
                CommonUITools.Widget.MessageBox.Error("不是合法数字！");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查合并文件输入有效性
        /// </summary>
        /// <returns></returns>
        private bool CheckMergeFileInputValidation() {
            if (string.IsNullOrEmpty(MergeFileSavePath)) {
                CommonUITools.Widget.MessageBox.Info("请输入文件保存路径！");
                return false;
            }
            if (string.IsNullOrEmpty(MergeFileDirectory)) {
                CommonUITools.Widget.MessageBox.Info("请输入合并文件目录！");
                return false;
            }
            if (!Directory.Exists(MergeFileDirectory)) {
                CommonUITools.Widget.MessageBox.Error("合并文件目录不存在！");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 选择合并文件保存路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectMergeFileSaveClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            var dialog = new SaveFileDialog {
                Filter = "All Files|*.*"
            };
            if (dialog.ShowDialog() != true) {
                return;
            }
            MergeFileSavePath = dialog.FileName;
        }

        /// <summary>
        /// 选择合并文件目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectMergeFileDirClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            var dialog = new VistaFolderBrowserDialog {
                Description = "选择合并文件夹",
                UseDescriptionForTitle = true // This applies to the Vista style dialog only, not the old dialog.
            };
            if (dialog.ShowDialog(Application.Current.MainWindow) == true) {
                MergeFileDirectory = dialog.SelectedPath;
                try {
                    MergeFiles = new(GetSortedFileList(new DirectoryInfo(MergeFileDirectory).GetFiles().Select(f => f.Name)));
                } catch (Exception error) {
                    Logger.Error(error);
                }
            }
        }

        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MergeFileClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            if (IsMergingFile) {
                CommonUITools.Widget.MessageBox.Info("正在合并文件");
                return;
            }
            if (!CheckMergeFileInputValidation()) {
                return;
            }
            string[] files = GetSortedFileList(
                                new DirectoryInfo(MergeFileDirectory).GetFiles().Select(f => f.Name)
                             ).ToArray();
            if (files.Length == 0) {
                CommonUITools.Widget.MessageBox.Error("合并文件为空！");
                return;
            }
            for (int i = 0; i < files.Length; i++) {
                files[i] = Path.Combine(MergeFileDirectory, files[i]);
            }
            string savePath = MergeFileSavePath;
            Task.Run(() => {
                try {
                    IsMergingFile = true;
                    MergeFileProcessTimer.Start();
                    FileMergeSplit.MergeFile(files, savePath, MergeFileProcessMonitor);
                    // 等待一段时间后再停止更新
                    ThreadPool.QueueUserWorkItem(p => {
                        Thread.Sleep((int)(MergeFileProcessTimer.Interval * 2));
                        MergeFileProcessTimer.Stop();
                    });
                } catch (Exception error) {
                    Logger.Error(error);
                    CommonUITools.Widget.MessageBox.Error("合并文件失败！");
                } finally {
                    IsMergingFile = false;
                }
                Dispatcher.Invoke(() => {
                    CommonUITools.Widget.MessageBox.Success("合并成功！");
                });
            });
        }

        /// <summary>
        /// 对文件进行排序
        /// </summary>
        /// <param name="filenameList"></param>
        /// <returns></returns>
        private IEnumerable<string> GetSortedFileList(IEnumerable<string> filenameList) {
            string prefix = CommonUtils.GetSamePrefix(filenameList);
            if (string.IsNullOrEmpty(prefix)) {
                return filenameList;
            }

            var fileDict = new Dictionary<int, string>();
            var regex = new Regex($@"{prefix}(\d+)(?:\..+)*");
            // 填充 fileDict
            foreach (var file in filenameList) {
                var match = regex.Match(file);
                if (match.Success) {
                    try {
                        fileDict[Convert.ToInt32(match.Groups[1].Value)] = file;
                    } catch {
                    }
                }
            }
            var keys = fileDict.Keys.ToList();
            keys.Sort();
            var resultList = new List<string>();
            foreach (var n in keys) {
                resultList.Add(fileDict[n]);
            }
            return resultList;
        }

    }
}
