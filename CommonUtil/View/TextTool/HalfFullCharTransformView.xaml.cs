using CommonUtil.Core;
using NLog;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View {
    public partial class HalfFullCharTransformView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(HalfFullCharTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(HalfFullCharTransformView), new PropertyMetadata(""));

        /// <summary>
        /// 输入文本
        /// </summary>
        public string InputText {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }
        /// <summary>
        /// 输出文本
        /// </summary>
        public string OutputText {
            get { return (string)GetValue(OutputTextProperty); }
            set { SetValue(OutputTextProperty, value); }
        }

        public HalfFullCharTransformView() {
            InitializeComponent();
        }

        /// <summary>
        /// 复制结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyResultClick(object sender, RoutedEventArgs e) {
            Clipboard.SetDataObject(OutputText);
            Widget.MessageBox.Success("已复制");
        }

        /// <summary>
        /// 清空输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearInputClick(object sender, RoutedEventArgs e) {
            InputText = string.Empty;
            OutputText = string.Empty;
        }

        /// <summary>
        /// 半角转全角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HalfToFullCharClick(object sender, RoutedEventArgs e) {
            OutputText = TextTool.HalfCharToFullChar(InputText);
        }

        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullToHalfCharClick(object sender, RoutedEventArgs e) {
            OutputText = TextTool.FullCharToHalfChar(InputText);
        }
    }
}
