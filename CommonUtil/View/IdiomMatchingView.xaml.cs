using CommonUtil.Core;
using ModernWpf.Controls;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CommonUtil.View {
    public partial class IdiomMatchingView : System.Windows.Controls.Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty InputIdiomProperty = DependencyProperty.Register("InputIdiom", typeof(string), typeof(IdiomMatchingView), new PropertyMetadata(""));
        public static readonly DependencyProperty MatchListProperty = DependencyProperty.Register("MatchList", typeof(List<string>), typeof(IdiomMatchingView), new PropertyMetadata());

        /// <summary>
        /// 输入 idiom
        /// </summary>
        public string InputIdiom {
            get { return (string)GetValue(InputIdiomProperty); }
            set { SetValue(InputIdiomProperty, value); }
        }
        /// <summary>
        /// 匹配列表
        /// </summary>
        public List<string> MatchList {
            get { return (List<string>)GetValue(MatchListProperty); }
            set { SetValue(MatchListProperty, value); }
        }

        public IdiomMatchingView() {
            InitializeComponent();
        }

        /// <summary>
        /// 生成点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateClick(object sender, RoutedEventArgs e) {
            GenerateMatchList();
        }

        /// <summary>
        /// 生成成语接龙
        /// </summary>
        private void GenerateMatchList() {
            if (InputIdiom.Trim() == string.Empty) {
                Widget.MessageBox.Info("请输入文本");
                return;
            }
            string idiom = InputIdiom.Trim();
            Task.Run(() => {
                List<List<string>> matches = IdiomMatching.GetMatchList(idiom);
                var list = new List<string>(matches.Count);
                foreach (var item in matches) {
                    list.Add(string.Join(" => ", item));
                }
                Dispatcher.Invoke(() => MatchList = list);
            });
        }

        /// <summary>
        /// 输入改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void InputIdiomChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            if (sender.Text.Trim() != string.Empty) {
                sender.ItemsSource = IdiomMatching.IdiomList
                    .Where(s => s.Contains(sender.Text))
                    .Take(8);
            }
        }

        /// <summary>
        /// 键盘按下 Enter 触发查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IdiomInputBoxKeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                GenerateMatchList();
            }
        }

        /// <summary>
        /// 复制当前行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyCurrentResultClick(object sender, RoutedEventArgs e) {
            if(sender is FrameworkElement element) {
                Clipboard.SetDataObject(element.DataContext);
                Widget.MessageBox.Success("已复制");
            }
        }

        /// <summary>
        /// 复制所有行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyAllResultsClick(object sender, RoutedEventArgs e) {
            if(ResultListBox.ItemsSource is IEnumerable<string> list) {
                var sb = new StringBuilder();
                foreach (var item in list) { 
                    sb.AppendLine(item);
                }
                Clipboard.SetDataObject(sb.ToString());
                Widget.MessageBox.Success("已复制");
            }
        }
    }
}
