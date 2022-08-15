using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View;

public partial class FileSplitView : Page {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 更新进度间隔时间
    /// </summary>
    private const int UpdateWorkingProcessInterval = 250;
    public static readonly DependencyProperty IsSizeOptionSelectedProperty = DependencyProperty.Register("IsSizeOptionSelected", typeof(bool), typeof(FileSplitView), new PropertyMetadata(true));
    public static readonly DependencyProperty SplitSizeOptionInputTextProperty = DependencyProperty.Register("SplitSizeOptionInputText", typeof(string), typeof(FileSplitView), new PropertyMetadata("100"));
    public static readonly DependencyProperty SplitNumberOptionInputTextProperty = DependencyProperty.Register("SplitNumberOptionInputText", typeof(string), typeof(FileSplitView), new PropertyMetadata("10"));
    public static readonly DependencyProperty SplitFilePathProperty = DependencyProperty.Register("SplitFilePath", typeof(string), typeof(FileSplitView), new PropertyMetadata(""));
    public static readonly DependencyProperty SplitFileSizeProperty = DependencyProperty.Register("SplitFileSize", typeof(long), typeof(FileSplitView), new PropertyMetadata(0L));
    public static readonly DependencyProperty SplitFileSaveDirectoryProperty = DependencyProperty.Register("SplitFileSaveDirectory", typeof(string), typeof(FileSplitView), new PropertyMetadata(""));
    public static readonly DependencyProperty FileSizeOptionsProperty = DependencyProperty.Register("FileSizeOptions", typeof(List<string>), typeof(FileSplitView), new PropertyMetadata());
    public static readonly DependencyProperty WorkingProcessProperty = DependencyProperty.Register("WorkingProcess", typeof(double), typeof(FileSplitView), new PropertyMetadata(0.0));
    public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register("IsWorking", typeof(bool), typeof(FileSplitView), new PropertyMetadata(false));

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
    private DateTime LastSplitFileUpdateTime = DateTime.Now;

    public FileSplitView() {
        FileSizeOptions = new() {
            "KB",
            "MB",
            "GB",
            "% 百分比",
        };
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
        if (IsWorking) {
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

        IsWorking = true;
        WorkingProcess = 0;
        string filepath = SplitFilePath;
        string saveDir = SplitFileSaveDirectory;
        // 开始分割
        try {
            await Task.Run(() => FileMergeSplit.SplitFile(
                filepath,
                saveDir,
                perSize,
                process => {
                    if ((DateTime.Now - LastSplitFileUpdateTime).TotalMilliseconds > UpdateWorkingProcessInterval) {
                        LastSplitFileUpdateTime = DateTime.Now;
                        Dispatcher.Invoke(() => WorkingProcess = process);
                    }
                })
            );
            MessageBox.Success("分割完成！");
        } catch (Exception error) {
            MessageBox.Error($"分割失败：{error.Message}");
        }
        IsWorking = false;
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


}
