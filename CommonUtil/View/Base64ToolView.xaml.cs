using CommonUtil.Core;
using Microsoft.Win32;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;
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
            e.Handled = true;
            if (!CheckInputValidation()) {
                return;
            }
            string inputText = InputText;
            Task.Run(() => {
                byte[]? result = Base64Tool.TryDecode(inputText);
                // 让文件保存对话框是一个 Modal
                Dispatcher.Invoke(() => {
                    if (result != null) {
                        var openFileDialog = new SaveFileDialog() {
                            Title = "保存文件",
                            Filter = "All Files|*.*"
                        };
                        if (openFileDialog.ShowDialog() == true) {
                            Task.Run(() => {
                                try {
                                    File.WriteAllBytes(openFileDialog.FileName, result);
                                    CommonUITools.Widget.MessageBox.Success("保存成功！");
                                } catch (Exception error) {
                                    CommonUITools.Widget.MessageBox.Error("保存失败");
                                    Logger.Info(error);
                                }
                            });
                        }
                        return;
                    }
                    CommonUITools.Widget.MessageBox.Error($"解码失败");
                });
            });
        }

        /// <summary>
        /// 编码文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncodeFile(object sender, RoutedEventArgs e) {
            e.Handled = true;
            var openFileDialog = new OpenFileDialog() {
                Title = "选择文件",
                Filter = "All Files|*.*"
            };
            if (openFileDialog.ShowDialog() == true) {
                Task.Run(() => {
                    try {
                        string result = Base64Tool.Base64Encode(openFileDialog.FileName);
                        Dispatcher.Invoke(() => OutputText = result);
                    } catch (IOException error) {
                        CommonUITools.Widget.MessageBox.Error("文件读取失败");
                        Logger.Info(error);
                    } catch (Exception error) {
                        CommonUITools.Widget.MessageBox.Error("编码失败");
                        Logger.Info(error);
                    }
                });
            }
        }

        /// <summary>
        /// 字符串解码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecodeString(object sender, RoutedEventArgs e) {
            e.Handled = true;
            if (CheckInputValidation()) {
                string inputText = InputText;
                Task.Run(() => {
                    try {
                        string result = Base64Tool.Base64StringDecode(inputText);
                        Dispatcher.Invoke(() => OutputText = result);
                    } catch (Exception error) {
                        CommonUITools.Widget.MessageBox.Error("解码失败");
                        Logger.Info(error);
                    }
                });
            }
        }

        /// <summary>
        /// 字符串编码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncodeString(object sender, RoutedEventArgs e) {
            e.Handled = true;
            if (CheckInputValidation()) {
                string inputText = InputText;
                Task.Run(() => {
                    try {
                        string result = Base64Tool.Base64StringEncode(inputText);
                        Dispatcher.Invoke(() => OutputText = result);
                    } catch (Exception error) {
                        CommonUITools.Widget.MessageBox.Error("编码失败");
                        Logger.Info(error);
                    }
                });
            }
        }

        /// <summary>
        /// 复制结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyResultClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(OutputText);
            CommonUITools.Widget.MessageBox.Success("已复制");
        }

        /// <summary>
        /// 检查输入是否合法
        /// </summary>
        /// <returns></returns>
        private bool CheckInputValidation() {
            if (string.IsNullOrEmpty(InputText)) {
                CommonUITools.Widget.MessageBox.Info("请输入文本");
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
            e.Handled = true;
            InputText = string.Empty;
            OutputText = string.Empty;
        }
    }
}
