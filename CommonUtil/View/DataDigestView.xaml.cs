using CommonUtil.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommonUtil.View {
    public partial class DataDigestView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private delegate string DigestHandler(string digest);

        public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(DataDigestView), new PropertyMetadata(""));
        public static readonly DependencyProperty DigestOptionsProperty = DependencyProperty.Register("DigestOptions", typeof(List<string>), typeof(DataDigestView), new PropertyMetadata());
        public static readonly DependencyProperty SelectedDigestIndexProperty = DependencyProperty.Register("SelectedDigestIndex", typeof(int), typeof(DataDigestView), new PropertyMetadata(0));
        private static readonly DependencyProperty DigestInfoDictProperty = DependencyProperty.Register("DigestInfoDict", typeof(Dictionary<string, DigestInfo>), typeof(DataDigestView), new PropertyMetadata());

        /// <summary>
        /// 输入文本
        /// </summary>
        public string InputText {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }
        /// <summary>
        /// 散列算法选择
        /// </summary>
        public List<string> DigestOptions {
            get { return (List<string>)GetValue(DigestOptionsProperty); }
            set { SetValue(DigestOptionsProperty, value); }
        }
        /// <summary>
        /// 选中的 Digest 算法
        /// </summary>
        public int SelectedDigestIndex {
            get { return (int)GetValue(SelectedDigestIndexProperty); }
            set { SetValue(SelectedDigestIndexProperty, value); }
        }
        /// <summary>
        /// 摘要算法 Dict
        /// </summary>
        private Dictionary<string, DigestInfo> DigestInfoDict {
            get { return (Dictionary<string, DigestInfo>)GetValue(DigestInfoDictProperty); }
            set { SetValue(DigestInfoDictProperty, value); }
        }

        public DataDigestView() {
            DigestOptions = new() {
                "全部",
                "MD2",
                "MD4",
                "MD5",
                "SHA1",
                "SHA3",
                "SHA224",
                "SHA256",
                "SHA384",
                "SHA512",
            };
            DigestInfoDict = new() {
                { "MD2", new() { DigestHandler = DataDigest.MD2Digest } },
                { "MD4", new() { DigestHandler = DataDigest.MD4Digest } },
                { "MD5", new() { DigestHandler = DataDigest.MD5Digest } },
                { "SHA1", new() { DigestHandler = DataDigest.SHA1Digest } },
                { "SHA3", new() { DigestHandler = DataDigest.SHA3Digest } },
                { "SHA224", new() { DigestHandler = DataDigest.SHA224Digest } },
                { "SHA256", new() { DigestHandler = DataDigest.SHA256Digest } },
                { "SHA384", new() { DigestHandler = DataDigest.SHA384Digest } },
                { "SHA512", new() { DigestHandler = DataDigest.SHA512Digest } },
            };
            InitializeComponent();
        }

        /// <summary>
        /// 清空输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearInputClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            InputText = string.Empty;
        }

        /// <summary>
        /// 开始计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            if (SelectedDigestIndex != 0) {
                var target = DigestInfoDict[DigestOptions[SelectedDigestIndex]];
                // 隐藏其他
                foreach (var item in DigestInfoDict.Values) {
                    if (item != target) {
                        item.IsVivible = false;
                    }
                }
                target.IsVivible = true;
                target.Text = target.DigestHandler.Invoke(InputText);
                return;
            }
            // 全部计算
            foreach (var item in DigestInfoDict.Values) {
                item.IsVivible = true;
                item.Text = item.DigestHandler.Invoke(InputText);
            }
        }

        /// <summary>
        /// 复制结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyResultClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            var sb = new StringBuilder();
            foreach (var item in DigestInfoDict) {
                if (item.Value.IsVivible) {
                    sb.Append($"{item.Key}: {item.Value.Text}\n");
                }
            }
            Clipboard.SetDataObject(sb.ToString());
            Widget.MessageBox.Success("已复制");
        }

        private class DigestInfo : DependencyObject {
            /// <summary>
            /// 处理器
            /// </summary>
            public DigestHandler DigestHandler { get; set; }
            /// <summary>
            /// 是否可见
            /// </summary>
            public bool IsVivible {
                get { return (bool)GetValue(IsVivibleProperty); }
                set { SetValue(IsVivibleProperty, value); }
            }
            public static readonly DependencyProperty IsVivibleProperty = DependencyProperty.Register("IsVivible", typeof(bool), typeof(DigestInfo), new PropertyMetadata(false));
            /// <summary>
            /// 结果
            /// </summary>
            public string Text {
                get { return (string)GetValue(TextProperty); }
                set { SetValue(TextProperty, value); }
            }
            public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DigestInfo), new PropertyMetadata(""));
        }

    }
}
