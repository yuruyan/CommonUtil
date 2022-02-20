using ModernWpf.Controls;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace CommonUtil.View {
    public partial class CommonRegexListDialog : ContentDialog {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly List<KeyValuePair<string, string>> CommonRegexList = new();
        private static readonly string CommonRegexPath = "resource/CommonRegex.json";

        /// <summary>
        /// 加载 CommonRegexList
        /// </summary>
        static CommonRegexListDialog() {
            try {
                var list = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(File.ReadAllText(CommonRegexPath));
                if (list != null) {
                    CommonRegexList = list;
                }
            } catch (FileNotFoundException) {
                Logger.Error("cannot find the file " + CommonRegexPath);
            } catch (Exception e) {
                Logger.Error(e);
            }
        }

        /// <summary>
        /// 正则列表
        /// </summary>
        public List<KeyValuePair<string, string>> RegexList {
            get { return (List<KeyValuePair<string, string>>)GetValue(RegexListProperty); }
            set { SetValue(RegexListProperty, value); }
        }
        public static readonly DependencyProperty RegexListProperty = DependencyProperty.Register("RegexList", typeof(List<KeyValuePair<string, string>>), typeof(CommonRegexListDialog), new PropertyMetadata());

        public CommonRegexListDialog() {
            RegexList = CommonRegexList;
            InitializeComponent();
        }
    }
}
