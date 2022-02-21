using CommonUtil.Core;
using NLog;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View {
    public partial class ChineseTransformView : Page {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(ChineseTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(ChineseTransformView), new PropertyMetadata(""));

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

        public ChineseTransformView() {
            InitializeComponent();
        }

        /// <summary>
        /// 转简体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToSimplifiedClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            OutputText = ChineseTransform.ToSimplified(InputText);
        }

        /// <summary>
        /// 转繁体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToTraditionalClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            OutputText = ChineseTransform.ToTraditional(InputText);
        }

        /// <summary>
        /// 复制结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyResultClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(OutputText);
            Widget.MessageBox.Success("已复制");
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
