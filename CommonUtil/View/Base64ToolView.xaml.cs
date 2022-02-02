using CommonUtil.Core;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View {
    public partial class Base64ToolView : Page {
        public Base64ToolView() {
            InitializeComponent();
        }

        private void DecodeImage(object sender, RoutedEventArgs e) {

        }

        private void EncodeImage(object sender, RoutedEventArgs e) {

        }

        /// <summary>
        /// 字符串解码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecodeString(object sender, RoutedEventArgs e) {
            if (CheckInputValidation()) {
                OutputTextBox.Text = Base64Tool.Base64StringDecode(InputTextBox.Text);
            }
        }

        /// <summary>
        /// 字符串编码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncodeString(object sender, RoutedEventArgs e) {
            if (CheckInputValidation()) {
                OutputTextBox.Text = Base64Tool.Base64StringEncode(InputTextBox.Text);
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
