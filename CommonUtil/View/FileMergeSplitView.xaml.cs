using CommonUITools.Utils;
using CommonUtil.Core;
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
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class FileMergeSplitView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 更新进度间隔时间
    /// </summary>
    private const int UpdateProcessInterval = 250;

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
    public static readonly DependencyProperty SplitProcessProperty = DependencyProperty.Register("SplitProcess", typeof(double), typeof(FileMergeSplitView), new PropertyMetadata(0.0));
    public static readonly DependencyProperty MergeProcessProperty = DependencyProperty.Register("MergeProcess", typeof(double), typeof(FileMergeSplitView), new PropertyMetadata(0.0));
    public static readonly DependencyProperty IsSplitingFileProperty = DependencyProperty.Register("IsSplitingFile", typeof(bool), typeof(FileMergeSplitView), new PropertyMetadata(false));
    public static readonly DependencyProperty IsMergingFileProperty = DependencyProperty.Register("IsMergingFile", typeof(bool), typeof(FileMergeSplitView), new PropertyMetadata(false));

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
    /// 分割文件进度
    /// </summary>
    public double SplitProcess {
        get { return (double)GetValue(SplitProcessProperty); }
        set { SetValue(SplitProcessProperty, value); }
    }
    /// <summary>
    /// 合并文件进度
    /// </summary>
    public double MergeProcess {
        get { return (double)GetValue(MergeProcessProperty); }
        set { SetValue(MergeProcessProperty, value); }
    }
    /// <summary>
    /// 是否正在分割文件
    /// </summary>
    public bool IsSplitingFile {
        get { return (bool)GetValue(IsSplitingFileProperty); }
        set { SetValue(IsSplitingFileProperty, value); }
    }
    /// <summary>
    /// 是否正在合并文件
    /// </summary>
    public bool IsMergingFile {
        get { return (bool)GetValue(IsMergingFileProperty); }
        set { SetValue(IsMergingFileProperty, value); }
    }
    /// <summary>
    /// 上次分割文件更新时间
    /// </summary>
    private DateTime LastSplitFileUpdateTime = DateTime.Now;
    /// <summary>
    /// 上次合并文件更新时间
    /// </summary>
    private DateTime LastMergeFileUpdateTime = DateTime.Now;

    public FileMergeSplitView() {
        FileSizeOptions = new() {
            "KB",
            "MB",
            "GB",
            "% 百分比",
        };
        MergeFiles = new();
        InitializeComponent();
    }

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
            MessageBox.Error("文件不存在！");
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
    private async void SplitFileClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (IsSplitingFile) {
            MessageBox.Info("正在分割文件");
            return;
        }
        if (!CheckSplitFileInputValidation()) {
            return;
        }
        ulong perSize = 0;
        if (IsSizeOptionSelected) {
            double size = 0;
            try {
                size = Convert.ToDouble(SplitSizeOptionInputText);
                if (size <= 0) {
                    throw new OverflowException("输入无效！");
                }
            } catch (Exception error) {
                Logger.Error(error);
                MessageBox.Error("输入无效！");
                return;
            }
            string? value = FileSizeOptionComboBox.SelectedValue.ToString();
            if (value == null) {
                Logger.Error("选择错误");
                MessageBox.Error("选择错误！");
                return;
            }
            if (value == "KB") {
                perSize = (ulong)(size * 0x400);
            } else if (value == "MB") {
                perSize = (ulong)(size * 0x100000);
            } else if (value == "GB") {
                perSize = (ulong)(size * 0x40000000);
            } else if (value.Contains('%')) {
                perSize = (ulong)(SplitFileSize * (size / 100));
            }
        } else {
            try {
                perSize = (ulong)(SplitFileSize / Convert.ToUInt16(SplitNumberOptionInputText));
            } catch (Exception error) {
                Logger.Error(error);
                MessageBox.Error("输入无效！");
                return;
            }
        }
        if (perSize <= 1) {
            MessageBox.Error("输入无效！");
            return;
        }

        IsSplitingFile = true;
        SplitProcess = 0;
        string filepath = SplitFilePath;
        string saveDir = SplitFileSaveDirectory;
        // 开始分割
        try {
            await Task.Run(() => FileMergeSplit.SplitFile(
                filepath,
                saveDir,
                perSize,
                process => {
                    if ((DateTime.Now - LastSplitFileUpdateTime).TotalMilliseconds > UpdateProcessInterval) {
                        LastSplitFileUpdateTime = DateTime.Now;
                        Dispatcher.Invoke(() => SplitProcess = process);
                    }
                })
            );
            MessageBox.Success("分割完成！");
        } catch (Exception error) {
            MessageBox.Error($"分割失败：{error.Message}");
        }
        IsSplitingFile = false;
    }

    /// <summary>
    /// 检查分割文件输入有效性
    /// </summary>
    /// <returns></returns>
    private bool CheckSplitFileInputValidation() {
        if (string.IsNullOrEmpty(SplitFilePath)) {
            MessageBox.Info("请选择要分割的文件！");
            return false;
        }
        if (string.IsNullOrEmpty(SplitFileSaveDirectory)) {
            MessageBox.Info("请选择保存目录！");
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
            MessageBox.Error("不是合法数字！");
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
            MessageBox.Info("请输入文件保存路径！");
            return false;
        }
        if (string.IsNullOrEmpty(MergeFileDirectory)) {
            MessageBox.Info("请输入合并文件目录！");
            return false;
        }
        if (!Directory.Exists(MergeFileDirectory)) {
            MessageBox.Error("合并文件目录不存在！");
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
    private async void MergeFileClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (IsMergingFile) {
            MessageBox.Info("正在合并文件");
            return;
        }
        if (!CheckMergeFileInputValidation()) {
            return;
        }
        string[] files = GetSortedFileList(
                            new DirectoryInfo(MergeFileDirectory).GetFiles().Select(f => f.Name)
                         ).ToArray();
        if (files.Length == 0) {
            MessageBox.Error("合并文件为空！");
            return;
        }
        for (int i = 0; i < files.Length; i++) {
            files[i] = Path.Combine(MergeFileDirectory, files[i]);
        }
        string savePath = MergeFileSavePath;
        IsMergingFile = true;
        MergeProcess = 0;
        try {
            await Task.Run(() => FileMergeSplit.MergeFile(
                files,
                savePath,
                process => {
                    if ((DateTime.Now - LastMergeFileUpdateTime).TotalMilliseconds > UpdateProcessInterval) {
                        LastMergeFileUpdateTime = DateTime.Now;
                        Dispatcher.Invoke(() => MergeProcess = process);
                    }
                }
            ));
            MessageBox.Success("合并完成");
        } catch (Exception error) {
            MessageBox.Error($"合并文件失败：{error.Message}");
        }
        IsMergingFile = false;
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

