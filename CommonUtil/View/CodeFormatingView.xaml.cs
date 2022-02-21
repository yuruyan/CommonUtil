using CommonUtil.Core;
using CommonUtil.Store;
using CommonUtil.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View {
    public partial class CodeFormatingView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(CodeFormatingView), new PropertyMetadata(""));
        public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(CodeFormatingView), new PropertyMetadata(""));
        public static readonly DependencyProperty LanguagesProperty = DependencyProperty.Register("Languages", typeof(List<string>), typeof(CodeFormatingView), new PropertyMetadata());
        public static readonly DependencyProperty SelectedLanguageIndexProperty = DependencyProperty.Register("SelectedLanguageIndex", typeof(int), typeof(CodeFormatingView), new PropertyMetadata(2));

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
        /// 支持的语言
        /// </summary>
        public List<string> Languages {
            get { return (List<string>)GetValue(LanguagesProperty); }
            set { SetValue(LanguagesProperty, value); }
        }
        /// <summary>
        /// 选择的语言 index
        /// </summary>
        public int SelectedLanguageIndex {
            get { return (int)GetValue(SelectedLanguageIndexProperty); }
            set { SetValue(SelectedLanguageIndexProperty, value); }
        }

        public CodeFormatingView() {
            Languages = new(CodeFormating.LanguageDict.Keys);
            InitializeComponent();
            // 启动 nodejs 服务
            Task.Run(() => CommonUtils.Try(() => Server.CheckNodeJsServer()));
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
        /// 格式化 Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormatClick(object sender, RoutedEventArgs e) {
            string code = InputText;
            CodeFormating.Lang lang = CodeFormating.LanguageDict[Languages[SelectedLanguageIndex]];
            Task.Run(async () => {
                try {
                    string formatedCode = await CodeFormating.FormatAsync(code, lang);
                    Dispatcher.Invoke(() => OutputText = formatedCode);
                } catch (Exception error) {
                    Dispatcher.Invoke(() => Widget.MessageBox.Error("格式化失败 " + error.Message));
                    Logger.Error(error);
                }
            });
        }
    }
}
