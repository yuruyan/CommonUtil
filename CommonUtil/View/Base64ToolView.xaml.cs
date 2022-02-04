using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View {
    public partial class Base64ToolView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(Base64ToolView), new PropertyMetadata(""));
        public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(Base64ToolView), new PropertyMetadata(""));
        
        /// <summary>
        /// 输入
        /// </summary>
        public string InputText {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }
        /// <summary>
        /// 输出结果
        /// </summary>
        public string OutputText {
            get { return (string)GetValue(OutputTextProperty); }
            set { SetValue(OutputTextProperty, value); }
        }

        public Base64ToolView() {
            InitializeComponent();
        }

        /// <summary>
        /// 解码图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecodeFile(object sender, RoutedEventArgs e) {
            if (CheckInputValidation()) {
                byte[]? result = Base64Tool.TryDecode(InputText);
                if (result != null) {
                    var openFileDialog = new OpenFileDialog() {
                        Title = "保存文件",
                        Filter = "All Files|*.*"
                    };
                    if (openFileDialog.ShowDialog() == true) {
                        try {
                            File.WriteAllBytes(openFileDialog.FileName, result);
                            Widget.MessageBox.Success("保存成功！");
                        } catch (Exception error) {
                            Logger.Info(error);
                            Widget.MessageBox.Error($"保存失败，{error.Message}");
                        }
                    }
                    return;
                }
                Widget.MessageBox.Error($"解码失败");
            }
        }

        /// <summary>
        /// 编码文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncodeFile(object sender, RoutedEventArgs e) {
            var openFileDialog = new OpenFileDialog() {
                Title = "选择文件",
                Filter = "All Files|*.*"
            };
            if (openFileDialog.ShowDialog() == true) {
                try {
                    OutputText = Base64Tool.Base64Encode(openFileDialog.FileName);
                } catch (Exception error) {
                    Logger.Info(error);
                    Widget.MessageBox.Error($"编码失败，{error.Message}");
                }
            }
        }

        /// <summary>
        /// 字符串解码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecodeString(object sender, RoutedEventArgs e) {
            if (CheckInputValidation()) {
                try {
                    OutputText = Base64Tool.Base64StringDecode(InputText);
                } catch (Exception error) {
                    Logger.Info(error);
                    Widget.MessageBox.Error($"解码失败，{error.Message}");
                }
            }
        }

        /// <summary>
        /// 字符串编码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncodeString(object sender, RoutedEventArgs e) {
            if (CheckInputValidation()) {
                try {
                    OutputText = Base64Tool.Base64StringEncode(InputText);
                } catch (Exception error) {
                    Logger.Info(error);
                    Widget.MessageBox.Error($"编码失败，{error.Message}");
                }
            }
        }

        /// <summary>
        /// 复制结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyResultClick(object sender, RoutedEventArgs e) {
            Clipboard.SetText(OutputText);
            Widget.MessageBox.Success("已复制");
        }

        /// <summary>
        /// 检查输入是否合法
        /// </summary>
        /// <returns></returns>
        private bool CheckInputValidation() {
            if (string.IsNullOrEmpty(InputText)) {
                Widget.MessageBox.Info("请输入文本");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 清空输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearInputClick(object sender, RoutedEventArgs e) {
            InputText = string.Empty;
        }
    }
}
