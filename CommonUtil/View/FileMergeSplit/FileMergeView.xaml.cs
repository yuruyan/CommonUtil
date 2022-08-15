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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class FileMergeView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 更新进度间隔时间
    /// </summary>
    private const int UpdateProcessInterval = 250;
    public static readonly DependencyProperty MergeFileDirectoryProperty = DependencyProperty.Register("MergeFileDirectory", typeof(string), typeof(FileMergeView), new PropertyMetadata(""));
    public static readonly DependencyProperty MergeFileSavePathProperty = DependencyProperty.Register("MergeFileSavePath", typeof(string), typeof(FileMergeView), new PropertyMetadata(""));
    public static readonly DependencyProperty MergeFilesProperty = DependencyProperty.Register("MergeFiles", typeof(ObservableCollection<string>), typeof(FileMergeView), new PropertyMetadata());
    public static readonly DependencyProperty WorkingProcessProperty = DependencyProperty.Register("WorkingProcess", typeof(double), typeof(FileMergeView), new PropertyMetadata(0.0));
    public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register("IsWorking", typeof(bool), typeof(FileMergeView), new PropertyMetadata(false));

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
    /// 合并文件进度
    /// </summary>
    public double WorkingProcess {
        get { return (double)GetValue(WorkingProcessProperty); }
        set { SetValue(WorkingProcessProperty, value); }
    }
    /// <summary>
    /// 是否正在合并文件
    /// </summary>
    public bool IsWorking {
        get { return (bool)GetValue(IsWorkingProperty); }
        set { SetValue(IsWorkingProperty, value); }
    }
    /// <summary>
    /// 上次合并文件更新时间
    /// </summary>
    private DateTime LastMergeFileUpdateTime = DateTime.Now;

    public FileMergeView() {
        MergeFiles = new();
        InitializeComponent();
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
        if (IsWorking) {
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
        IsWorking = true;
        WorkingProcess = 0;
        try {
            await Task.Run(() => FileMergeSplit.MergeFile(
                files,
                savePath,
                process => {
                    if ((DateTime.Now - LastMergeFileUpdateTime).TotalMilliseconds > UpdateProcessInterval) {
                        LastMergeFileUpdateTime = DateTime.Now;
                        Dispatcher.Invoke(() => WorkingProcess = process);
                    }
                }
            ));
            MessageBox.Success("合并完成");
        } catch (Exception error) {
            MessageBox.Error($"合并文件失败：{error.Message}");
        }
        IsWorking = false;
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
