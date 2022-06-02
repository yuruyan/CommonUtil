using CommonUtil.Core;
using CommonUtil.Model;
using NLog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommonUtil.View {
    public partial class AsciiTableView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty AsciiTableListProperty = DependencyProperty.Register("AsciiTableList", typeof(ObservableCollection<AsciiInfo>), typeof(AsciiTableView), new PropertyMetadata());
        /// <summary>
        /// ASCII 列表
        /// </summary>
        public ObservableCollection<AsciiInfo> AsciiTableList {
            get { return (ObservableCollection<AsciiInfo>)GetValue(AsciiTableListProperty); }
            set { SetValue(AsciiTableListProperty, value); }
        }

        public AsciiTableView() {
            AsciiTableList = new();
            InitializeComponent();
            // 加载数据
            ThreadPool.QueueUserWorkItem(o => {
                List<AsciiInfo> list = AsciiTable.GetAsciiInfoList();
                Dispatcher.Invoke(() => {
                    for (int i = 0; i < list.Count; i++) {
                        AsciiTableList.Add(list[i]);
                    }
                });
            });
        }

        /// <summary>
        /// 复制详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyDetailClickHandler(object sender, RoutedEventArgs e) {
            e.Handled = true;
            if (sender is FrameworkElement element) {
                if (element.DataContext is AsciiInfo info) {
                    Clipboard.SetDataObject($"{info.Binary}\t{info.Octal}\t{info.Decimal}\t{info.HexaDecimal}\t{info.Character}\t{info.HtmlEntity}\t{info.Description}");
                    CommonUITools.Widget.MessageBox.Success("已复制");
                }
            }
        }
    }
}
