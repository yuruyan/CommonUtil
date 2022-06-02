using CommonUtil.Core;
using NLog;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View {
    public partial class RemoveDuplicateView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RemoveDuplicateView), new PropertyMetadata(""));
        public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(RemoveDuplicateView), new PropertyMetadata(""));
        public static readonly DependencyProperty SymbolOptionsProperty = DependencyProperty.Register("SymbolOptions", typeof(List<string>), typeof(RemoveDuplicateView), new PropertyMetadata());

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
        /// 标记选项
        /// </summary>
        public List<string> SymbolOptions {
            get { return (List<string>)GetValue(SymbolOptionsProperty); }
            set { SetValue(SymbolOptionsProperty, value); }
        }
        private Dictionary<string, string> SymbolDict;

        public RemoveDuplicateView() {
            SymbolDict = new() {
                { "换行符（⮠  ）", "\n" },
                { "制表符（→）", "\t" },
                { "空格（ ）", " " },
                { "中文逗号（，）", "，" },
                { "英文逗号（,）", "," },
            };
            SymbolOptions = new(SymbolDict.Keys);
            InitializeComponent();
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
        /// 清空输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearInputClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            InputText = string.Empty;
            OutputText = string.Empty;
        }

        /// <summary>
        /// 文本去重
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveDuplicateClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            if (InputText == string.Empty) {
                CommonUITools.Widget.MessageBox.Info("请输入文本");
                return;
            }
            var splitSymbol = GetComboBoxText(SplitSymbolBox);
            var mergeSymbol = GetComboBoxText(MergeSymbolBox);
            OutputText = TextTool.RemoveDuplicate(InputText, splitSymbol, mergeSymbol, TrimWhiteSpaceCheckBox.IsChecked == true);
        }

        /// <summary>
        /// 获取 ComboBox 文本
        /// </summary>
        /// <param name="comboBox"></param>
        /// <returns></returns>
        private string GetComboBoxText(ComboBox comboBox) {
            object selectedValue = comboBox.SelectedValue;
            string text = comboBox.Text;
            // 非用户输入
            if (selectedValue != null) {
                if (selectedValue is string t) {
                    text = SymbolDict[t];
                }
            }
            return text;
        }
    }
}
