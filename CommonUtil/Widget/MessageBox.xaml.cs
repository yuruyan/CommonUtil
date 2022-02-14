using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CommonUtil.Widget {
    public partial class MessageBox : UserControl {

        private static readonly DependencyProperty BoxBackgroundProperty = DependencyProperty.Register("BoxBackground", typeof(string), typeof(MessageBox), new PropertyMetadata(""));
        private static readonly DependencyProperty BoxForegroundProperty = DependencyProperty.Register("BoxForeground", typeof(string), typeof(MessageBox), new PropertyMetadata(""));
        private static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(string), typeof(MessageBox), new PropertyMetadata(""));
        private static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(MessageBox), new PropertyMetadata(""));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MainWindow), new PropertyMetadata(""));
        public static readonly DependencyProperty MessageTypeProperty = DependencyProperty.Register("MessageType", typeof(MessageType), typeof(MessageBox), new PropertyMetadata(MessageType.INFO));

        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// Background
        /// </summary>
        private string BoxBackground {
            get { return (string)GetValue(BoxBackgroundProperty); }
            set { SetValue(BoxBackgroundProperty, value); }
        }
        /// <summary>
        /// Foreground
        /// </summary>
        private string BoxForeground {
            get { return (string)GetValue(BoxForegroundProperty); }
            set { SetValue(BoxForegroundProperty, value); }
        }
        /// <summary>
        /// BorderColor
        /// </summary>
        private string BorderColor {
            get { return (string)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
        /// <summary>
        /// Icon
        /// </summary>
        private string Icon {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        /// <summary>
        /// MessageType
        /// </summary>
        public MessageType MessageType {
            get { return (MessageType)GetValue(MessageTypeProperty); }
            set { SetValue(MessageTypeProperty, value); }
        }

        /// <summary>
        /// 显示时间 (ms)
        /// </summary>
        public int ShowingDuration { get; set; } = 3000;
        /// <summary>
        /// 用于添加 MessageBox
        /// </summary>
        public static UIElementCollection? PanelChildren;

        private static void ShowMessage(string message, MessageType type) {
            PanelChildren?.Add(new MessageBox(message, type));
        }

        public static void Info(string message) {
            ShowMessage(message, MessageType.INFO);
        }

        public static void Waring(string message) {
            ShowMessage(message, MessageType.WARNING);
        }

        public static void Success(string message) {
            ShowMessage(message, MessageType.SUCCESS);
        }

        public static void Error(string message) {
            ShowMessage(message, MessageType.ERROR);
        }

        public MessageBox(string message, MessageType messageType = MessageType.INFO) {
            Text = message;
            MessageType = messageType;
            BoxBackground = WidgetGlobal.MessageInfoDict[MessageType].Background;
            BoxForeground = WidgetGlobal.MessageInfoDict[MessageType].Foreground;
            BorderColor = WidgetGlobal.MessageInfoDict[MessageType].BorderColor;
            Icon = WidgetGlobal.MessageInfoDict[MessageType].Icon;
            InitializeComponent();
            Timer timer = new Timer(ShowingDuration) { AutoReset = false };
            timer.Elapsed += RootUnLoad;
            timer.Start();
        }

        /// <summary>
        /// 消失时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootUnLoad(object? sender, ElapsedEventArgs e) {
            Dispatcher.Invoke(() => {
                if (Resources["UnLoadStoryboard"] is Storyboard storyboard) {
                    var enumerable = from a in storyboard.Children
                                     where a.Name == "HeightAnimation"
                                     select a;
                    if (enumerable.Any()) {
                        if (enumerable.First() is DoubleAnimation heightAnimation) {
                            heightAnimation.From = ActualHeight;
                        }
                    }
                    storyboard.Completed += (s, e) => Visibility = Visibility.Collapsed;
                    storyboard.Begin();
                }
            });
        }

        /// <summary>
        /// 显示时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootLoaded(object sender, RoutedEventArgs e) {
            if (Parent is FrameworkElement parent) {
                if (Resources["LoadStoryboard"] is Storyboard storyboard) {
                    storyboard.Begin();
                }
            }
        }
    }
}
