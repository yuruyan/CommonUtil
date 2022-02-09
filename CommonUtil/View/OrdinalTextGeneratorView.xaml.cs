using CommonUtil.Core;
using ModernWpf.Controls;
using NLog;
using System;
using System.Text;
using System.Windows;

namespace CommonUtil.View {
    public partial class OrdinalTextGeneratorView : System.Windows.Controls.Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(OrdinalTextGeneratorView), new PropertyMetadata(""));
        public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(OrdinalTextGeneratorView), new PropertyMetadata(""));
        public static readonly DependencyProperty StartIndexProperty = DependencyProperty.Register("StartIndex", typeof(double), typeof(OrdinalTextGeneratorView), new PropertyMetadata(1.0));
        public static readonly DependencyProperty GenerationCountProperty = DependencyProperty.Register("GenerationCount", typeof(double), typeof(OrdinalTextGeneratorView), new PropertyMetadata(10.0));
        public static readonly DependencyProperty GenerationOrderProperty = DependencyProperty.Register("GenerationOrder", typeof(int), typeof(OrdinalTextGeneratorView), new PropertyMetadata(0));

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
        /// <summary>
        /// 起始位置
        /// </summary>
        public double StartIndex {
            get { return (double)GetValue(StartIndexProperty); }
            set { SetValue(StartIndexProperty, value); }
        }
        /// <summary>
        /// 生成数量
        /// </summary>
        public double GenerationCount {
            get { return (double)GetValue(GenerationCountProperty); }
            set { SetValue(GenerationCountProperty, value); }
        }
        /// <summary>
        /// 生成顺序
        /// </summary>
        public int GenerationOrder {
            get { return (int)GetValue(GenerationOrderProperty); }
            set { SetValue(GenerationOrderProperty, value); }
        }

        public OrdinalTextGeneratorView() {
            InitializeComponent();
            InputText = "abc{}";
            GenerateText();
        }

        /// <summary>
        /// 生成文本
        /// </summary>
        private void GenerateText() {
            var list = OrdinalTextGenerator.Generate(InputText, (int)StartIndex, (int)GenerationCount);
            var sb = new StringBuilder();
            // 逆序
            if (GenerationOrder != 0) {
                for (int i = list.Count - 1; i >= 0; i--) {
                    sb.AppendLine(list[i]);
                }
            } else {
                foreach (var item in list) {
                    sb.AppendLine(item);
                }
            }
            OutputText = sb.ToString();
        }

        /// <summary>
        /// 生成按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateClick(object sender, RoutedEventArgs e) {
            GenerateText();
        }

        private void NumberBoxLostFocus(object sender, RoutedEventArgs e) {
            // 浮点数转整数
            if (sender is NumberBox numberBox) {
                try {
                    numberBox.Value = (int)numberBox.Value;
                } catch {
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
        /// 清空输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearInputClick(object sender, RoutedEventArgs e) {
            InputText = string.Empty;
        }
    }
}
