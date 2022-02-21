using CommonUtil.Core;
using NLog;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommonUtil.View {
    public partial class RegexExtractionView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(RegexExtractionView), new PropertyMetadata(""));
        public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RegexExtractionView), new PropertyMetadata(""));
        public static readonly DependencyProperty SearchRegexProperty = DependencyProperty.Register("SearchRegex", typeof(string), typeof(RegexExtractionView), new PropertyMetadata(""));
        public static readonly DependencyProperty IgnoreCaseProperty = DependencyProperty.Register("IgnoreCase", typeof(bool), typeof(RegexExtractionView), new PropertyMetadata(true));
        public static readonly DependencyProperty MatchListProperty = DependencyProperty.Register("MatchList", typeof(List<string>), typeof(RegexExtractionView), new PropertyMetadata());

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
        /// 查找正则
        /// </summary>
        public string SearchRegex {
            get { return (string)GetValue(SearchRegexProperty); }
            set { SetValue(SearchRegexProperty, value); }
        }
        /// <summary>
        /// 忽略大小写
        /// </summary>
        public bool IgnoreCase {
            get { return (bool)GetValue(IgnoreCaseProperty); }
            set { SetValue(IgnoreCaseProperty, value); }
        }
        /// <summary>
        /// 匹配列表
        /// </summary>
        public List<string> MatchList {
            get { return (List<string>)GetValue(MatchListProperty); }
            set { SetValue(MatchListProperty, value); }
        }
        /// <summary>
        /// 常用正则表达式 Dialog
        /// </summary>
        private CommonRegexListDialog? CommonRegexListDialog;

        public RegexExtractionView() {
            InitializeComponent();
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
            OutputText = string.Empty;
        }

        /// <summary>
        /// 查找结果+
        /// </summary>
        private void SearchResult() {
            ResultDetailPanel.Visibility = Visibility.Visible;
            var list = RegexExtraction.Extract(SearchRegex, InputText, IgnoreCase);
            if (list == null) {
                Widget.MessageBox.Error("正则表达式有误");
                return;
            }
            MatchList = list;
            OutputText = string.Join('\n', list);
        }

        /// <summary>
        /// 点击查找
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchClick(object sender, RoutedEventArgs e) {
            SearchResult();
        }

        /// <summary>
        /// 按下回车查找
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchRegexComboBoxKeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                SearchResult();
            }
        }

        /// <summary>
        /// 显示更多正则表达式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoreRegexMouseUp(object sender, MouseButtonEventArgs e) {
            if (CommonRegexListDialog == null) {
                CommonRegexListDialog = new();
            }
            CommonRegexListDialog.ShowAsync();
        }
    }
}
