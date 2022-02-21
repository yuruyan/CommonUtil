using ModernWpf.Controls;
using NLog;
using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace CommonUtil.View {
    public partial class RandomGeneratorView : System.Windows.Controls.Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 输出
        /// </summary>
        public string OutputText {
            get { return (string)GetValue(OutputTextProperty); }
            set { SetValue(OutputTextProperty, value); }
        }
        public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RandomGeneratorView), new PropertyMetadata(""));

        private RandomStringGeneratorView RandomStringGeneratorViewInstance = new();
        private RandomNumberGeneratorView RandomNumberGeneratorViewInstance = new();

        public RandomGeneratorView() {
            InitializeComponent();
            NavigationView.SelectedItem = RandomStringGeneratorView;
            // 在 NavigationView.SelectedItem 设置之后
            NavigationView.SelectionChanged += NavigationViewSelectionChanged;
            ContentFrame.Navigate(RandomStringGeneratorViewInstance);
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyResultClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(OutputText);
            Widget.MessageBox.Success("已复制");
        }

        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            var methodInfos = ContentFrame.CurrentSourcePageType.GetMethods();
            var methodInfo = methodInfos.FirstOrDefault(m => m.Name == "Generate");
            if (methodInfo != null) {
                object? result = methodInfo.Invoke(null, null);
                if (result != null && result is Array array) {
                    var sb = new StringBuilder();
                    foreach (var s in array) {
                        sb.AppendLine(s.ToString());
                    }
                    OutputText = sb.ToString();
                }
            }
        }

        /// <summary>
        /// 导航变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
            if (args.SelectedItem is FrameworkElement element) {
                // 相同 Navigation
                if (element.Name == ContentFrame.CurrentSourcePageType.Name) {
                    return;
                }
                if (element.Name == "RandomNumberGeneratorView") {
                    ContentFrame.Navigate(RandomNumberGeneratorViewInstance);
                } else if (element.Name == "RandomStringGeneratorView") {
                    ContentFrame.Navigate(RandomStringGeneratorViewInstance);
                }
            }
        }

    }
}
