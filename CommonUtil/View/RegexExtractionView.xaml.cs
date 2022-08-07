﻿using CommonUtil.Core;
using NLog;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUtil.View {
    public partial class RegexExtractionView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(RegexExtractionView), new PropertyMetadata(""));
        public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(RegexExtractionView), new PropertyMetadata(""));
        public static readonly DependencyProperty SearchRegexProperty = DependencyProperty.Register("SearchRegex", typeof(string), typeof(RegexExtractionView), new PropertyMetadata(""));
        public static readonly DependencyProperty ExtractionPatternProperty = DependencyProperty.Register("ExtractionPattern", typeof(string), typeof(RegexExtractionView), new PropertyMetadata("\\0"));
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
        /// 提取模式
        /// </summary>
        public string ExtractionPattern {
            get { return (string)GetValue(ExtractionPatternProperty); }
            set { SetValue(ExtractionPatternProperty, value); }
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
            e.Handled = true;
            Clipboard.SetDataObject(OutputText);
            MessageBox.Success("已复制");
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
        /// 查找结果+
        /// </summary>
        private void SearchResult() {
            ResultDetailPanel.Visibility = Visibility.Visible;
            // 为空则不查找
            if (string.IsNullOrEmpty(SearchRegex)) {
                MessageBox.Info("输入不能为空");
                return;
            }
            var list = RegexExtraction.Extract(
                SearchRegex,
                InputText,
                ExtractionPattern,
                ignoreCase: IgnoreCase
            );
            if (list == null) {
                MessageBox.Error("正则表达式有误");
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
            e.Handled = true;
            SearchResult();
        }

        /// <summary>
        /// 按下回车查找
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchRegexComboBoxKeyUp(object sender, KeyEventArgs e) {
            e.Handled = true;
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
            e.Handled = true;
            if (CommonRegexListDialog == null) {
                CommonRegexListDialog = new();
            }
            CommonRegexListDialog.ShowAsync();
        }
    }
}
