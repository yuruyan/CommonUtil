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
using System.Windows.Shapes;

namespace CommonUtil.View {
    public partial class FileMergeSplitView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty IsSizeOptionSelectedProperty = DependencyProperty.Register("IsSizeOptionSelected", typeof(bool), typeof(FileMergeSplitView), new PropertyMetadata(true));
        public static readonly DependencyProperty SplitSizeOptionInputTextProperty = DependencyProperty.Register("SplitSizeOptionInputText", typeof(string), typeof(FileMergeSplitView), new PropertyMetadata("100"));
        public static readonly DependencyProperty SplitNumberOptionInputTextProperty = DependencyProperty.Register("SplitNumberOptionInputText", typeof(string), typeof(FileMergeSplitView), new PropertyMetadata("10"));
        public static readonly DependencyProperty SplitSelectedFileNameProperty = DependencyProperty.Register("SplitSelectedFileName", typeof(string), typeof(FileMergeSplitView), new PropertyMetadata(""));
        public static readonly DependencyProperty SplitSelectedFileSizeProperty = DependencyProperty.Register("SplitSelectedFileSize", typeof(string), typeof(FileMergeSplitView), new PropertyMetadata(""));
        public static readonly DependencyProperty SplitFileSaveDirectoryProperty = DependencyProperty.Register("SplitFileSaveDirectory", typeof(string), typeof(FileMergeSplitView), new PropertyMetadata(""));
        public static readonly DependencyProperty FileSizeOptionsProperty = DependencyProperty.Register("FileSizeOptions", typeof(List<string>), typeof(FileMergeSplitView), new PropertyMetadata());

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
        /// 分割文件名
        /// </summary>
        public string SplitSelectedFileName {
            get { return (string)GetValue(SplitSelectedFileNameProperty); }
            set { SetValue(SplitSelectedFileNameProperty, value); }
        }
        /// <summary>
        /// 分割文件大小
        /// </summary>
        public string SplitSelectedFileSize {
            get { return (string)GetValue(SplitSelectedFileSizeProperty); }
            set { SetValue(SplitSelectedFileSizeProperty, value); }
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
        /// 分割文件路径
        /// </summary>
        private string SplitFilePath = string.Empty;
        /// <summary>
        /// 分割文件大小
        /// </summary>
        private long SplitFileSize = 0;

        public FileMergeSplitView() {
            FileSizeOptions = new() {
                "KB",
                "MB",
                "GB",
                "%百分比",
            };
            InitializeComponent();

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
                SplitSelectedFileName = fileInfo.Name;
                if (fileInfo.Length < 1024) {
                    SplitSelectedFileSize = string.Format("{0} B", fileInfo.Length);
                } else if (fileInfo.Length < 1048576) {
                    SplitSelectedFileSize = string.Format("{0:F2} KB", fileInfo.Length / (double)1024);
                } else {
                    SplitSelectedFileSize = string.Format("{0:F2} MB", fileInfo.Length / (double)1048576);
                }
            } catch (Exception error) {
                Logger.Info(error);
                Widget.MessageBox.Error("文件不存在！");
            }
        }

        /// <summary>
        /// 选择分割文件保存目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFileSplitSaveDirClick(object sender, RoutedEventArgs e) {
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
            if (!CheckInputValidation()) {
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
                    Widget.MessageBox.Error("输入无效！");
                    return;
                }
                string? value = FileSizeOptionComboBox.SelectedValue.ToString();
                if (value == "KB") {
                    perSize = (long)(size * 0x400);
                } else if (value == "MB") {
                    perSize = (long)(size * 0x100000);
                } else if (value == "GB") {
                    perSize = (long)(size * 0x40000000);
                } else if (value.Contains("%")) {
                    perSize = (long)(SplitFileSize * (size / 100));
                }
            } else {
                try {
                    perSize = SplitFileSize / Convert.ToUInt16(SplitNumberOptionInputText);
                } catch (Exception error) {
                    Logger.Error(error);
                    Widget.MessageBox.Error("输入无效！");
                    return;
                }
            }
            if (perSize <= 1) {
                Widget.MessageBox.Error("输入无效！");
                return;
            }
            try {
                FileMergeSplit.SplitFile(SplitFilePath, SplitFileSaveDirectory, perSize);
                Widget.MessageBox.Success("分割完成！");
            } catch (Exception error) {
                Logger.Error(error);
                Widget.MessageBox.Error("分割失败！");
            }
        }

        /// <summary>
        /// 检查输入有效性
        /// </summary>
        /// <returns></returns>
        private bool CheckInputValidation() {
            if (string.IsNullOrEmpty(SplitFilePath)) {
                Widget.MessageBox.Error("请选择要分割的文件！");
                return false;
            }
            if (string.IsNullOrEmpty(SplitFileSaveDirectory)) {
                Widget.MessageBox.Error("请选择保存目录！");
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
                Widget.MessageBox.Error("不是合法数字！");
                return false;
            }
            return true;
        }

    }
}
