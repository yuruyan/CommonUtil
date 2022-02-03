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
                byte[]? result = Base64Tool.TryDecode(InputTextBox.Text);
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
                    OutputTextBox.Text = Base64Tool.Base64Encode(openFileDialog.FileName);
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
                    OutputTextBox.Text = Base64Tool.Base64StringDecode(InputTextBox.Text);
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
                    OutputTextBox.Text = Base64Tool.Base64StringEncode(InputTextBox.Text);
                } catch (Exception error) {
                    Logger.Info(error);
                    Widget.MessageBox.Error($"编码失败，{error.Message}");
                }
            }
        }

        /// <summary>
        /// 检查输入是否合法
        /// </summary>
        /// <returns></returns>
        private bool CheckInputValidation() {
            if (string.IsNullOrEmpty(InputTextBox.Text)) {
                Widget.MessageBox.Info("输入不能为空！");
                return false;
            }
            return true;
        }
    }
}
