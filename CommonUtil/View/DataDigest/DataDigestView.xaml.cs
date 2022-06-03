using CommonUITools.Utils;
using CommonUtil.Core;
using CommonUtil.Store;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommonUtil.View {
    public partial class DataDigestView : Page {

        private class DigestInfo : DependencyObject {
            /// <summary>
            /// 文本 Hash 处理器
            /// </summary>
            public TextDigestHandler TextDigestHandler { get; set; }
            /// <summary>
            /// 文件流 Hash 处理器
            /// </summary>
            public StreamDigestHandler StreamDigestHandler { get; set; }
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

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private delegate string TextDigestHandler(string digest);
        private delegate string StreamDigestHandler(FileStream stream);

        public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(DataDigestView), new PropertyMetadata(""));
        public static readonly DependencyProperty DigestOptionsProperty = DependencyProperty.Register("DigestOptions", typeof(List<string>), typeof(DataDigestView), new PropertyMetadata());
        public static readonly DependencyProperty SelectedDigestIndexProperty = DependencyProperty.Register("SelectedDigestIndex", typeof(int), typeof(DataDigestView), new PropertyMetadata(0));
        private static readonly DependencyProperty DigestInfoDictProperty = DependencyProperty.Register("DigestInfoDict", typeof(Dictionary<string, DigestInfo>), typeof(DataDigestView), new PropertyMetadata());
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(DataDigestView), new PropertyMetadata(""));
        public static readonly DependencyProperty RunningProcessProperty = DependencyProperty.Register("RunningProcess", typeof(int), typeof(DataDigestView), new PropertyMetadata(0));

        /// <summary>
        /// 输入文件名
        /// </summary>
        public string FileName {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }
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
        /// <summary>
        /// 当前进行的任务
        /// </summary>
        public int RunningProcess {
            get { return (int)GetValue(RunningProcessProperty); }
            set { SetValue(RunningProcessProperty, value); }
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
                { "MD2", new() { TextDigestHandler = DataDigest.MD2Digest, StreamDigestHandler = DataDigest.MD2Digest } },
                { "MD4", new() { TextDigestHandler = DataDigest.MD4Digest, StreamDigestHandler = DataDigest.MD4Digest } },
                { "MD5", new() { TextDigestHandler = DataDigest.MD5Digest, StreamDigestHandler = DataDigest.MD5Digest } },
                { "SHA1", new() { TextDigestHandler = DataDigest.SHA1Digest, StreamDigestHandler = DataDigest.SHA1Digest } },
                { "SHA3", new() { TextDigestHandler = DataDigest.SHA3Digest, StreamDigestHandler = DataDigest.SHA3Digest } },
                { "SHA224", new() { TextDigestHandler = DataDigest.SHA224Digest, StreamDigestHandler = DataDigest.SHA224Digest } },
                { "SHA256", new() { TextDigestHandler = DataDigest.SHA256Digest, StreamDigestHandler = DataDigest.SHA256Digest } },
                { "SHA384", new() { TextDigestHandler = DataDigest.SHA384Digest, StreamDigestHandler = DataDigest.SHA384Digest } },
                { "SHA512", new() { TextDigestHandler = DataDigest.SHA512Digest, StreamDigestHandler = DataDigest.SHA512Digest } },
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
            FileName = string.Empty;
            foreach (var item in DigestInfoDict) {
                item.Value.IsVivible = false;
            }
        }

        /// <summary>
        /// 开始计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            ThrottleUtils.ThrottleAsync(StartClick, async () => {
                await calculateDigest();
            });
        }

        /// <summary>
        /// 计算 Hash
        /// </summary>
        /// <returns></returns>
        private async Task calculateDigest() {
            if (SelectedDigestIndex != 0) {
                var target = DigestInfoDict[DigestOptions[SelectedDigestIndex]];
                // 隐藏其他
                foreach (var item in DigestInfoDict.Values) {
                    if (item != target) {
                        item.IsVivible = false;
                    }
                }
                await calculateDigest(new DigestInfo[] { target });
                return;
            }
            // 全部计算
            await calculateDigest(DigestInfoDict.Values);
        }

        /// <summary>
        /// 计算 Hash
        /// </summary>
        /// <param name="digests"></param>
        /// <returns></returns>
        private async Task calculateDigest(IEnumerable<DigestInfo> digests) {
            // 先清空
            foreach (var item in digests) {
                item.Text = string.Empty;
                RunningProcess++;
            }
            // 计算
            foreach (var item in digests) {
                item.IsVivible = true;
                var text = InputText;
                var filename = FileName;
                item.Text = await Task.Run(() => {
                    // 计算文本 Hash
                    if (string.IsNullOrEmpty(filename)) {
                        return item.TextDigestHandler.Invoke(text);
                    }
                    // 计算文件 Hash
                    try {
                        return item.StreamDigestHandler.Invoke(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
                    } catch (Exception e) {
                        CommonUITools.Widget.MessageBox.Error(e.Message);
                        return string.Empty;
                    }
                });
                RunningProcess--;
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
            CommonUITools.Widget.MessageBox.Success("已复制");
        }

        /// <summary>
        /// 拖放文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragFileDropHandler(object sender, DragEventArgs e) {
            e.Handled = true;
            if (e.Data.GetData(DataFormats.FileDrop) is IEnumerable<string> array) {
                if (!array.Any()) {
                    return;
                }
                FileName = array.First();
            }
        }

        /// <summary>
        /// 文件拖放进入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragFileEnterHandler(object sender, DragEventArgs e) {
            e.Handled = true;
            InputPanel.Background = (SolidColorBrush)Global.ColorResource["Gray3"];
            InputTextBox.Background = (SolidColorBrush)Global.ColorResource["Gray3"];
        }

        /// <summary>
        /// 更改 TextBox 默认拖拽行为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileDragOverHandler(object sender, DragEventArgs e) {
            e.Handled = true;
            e.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// 设置鼠标移除时清除背景
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputMouseLeaveHandler(object sender, System.Windows.Input.MouseEventArgs e) {
            e.Handled = true;
            InputPanel.Background = new SolidColorBrush(Colors.Transparent);
            InputTextBox.Background = new SolidColorBrush(Colors.Transparent);
        }
    }
}
