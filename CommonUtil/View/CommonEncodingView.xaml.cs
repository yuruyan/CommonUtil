using CommonUtil.Route;
using ModernWpf.Controls;
using NLog;
using System;
using System.Windows;

namespace CommonUtil.View {
    public partial class CommonEncodingView : System.Windows.Controls.Page {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Type[] Routers = {
                typeof(UnicodeEncodingView),
                typeof(UTF8EncodingView),
                typeof(URLEncodingView),
                typeof(HexEncodingView),
        };
        private RouterService RouterService;

        public CommonEncodingView() {
            InitializeComponent();
            RouterService = new(ContentFrame, Routers);
            //RouterService.Navigate(typeof(UnicodeEncodingView));
            //EncodingNavigationView.SelectionChanged += NavigationSelectionChanged;
        }

        /// <summary>
        /// 路由跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
            Console.WriteLine("navigation");
            if (args.SelectedItem is FrameworkElement element) {
                foreach (var item in Routers) {
                    if (item.Name.Contains(element.Name)) {
                        RouterService.Navigate(item);
                        break;
                    }
                }
            }
        }
    }
}