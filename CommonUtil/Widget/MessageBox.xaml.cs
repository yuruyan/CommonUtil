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

        private Dictionary<MessageType, MessageTypeData> MessageMap = new() {
            {
                MessageType.INFO,
                new MessageTypeData {
                    Background = "#F4F4F5",
                    Foreground = "#9D9399",
                    BorderColor = "#e5e5e6",
                    Icon = "\ue650"
                }
            },
            {
                MessageType.SUCCESS,
                new MessageTypeData {
                    Background = "#f0f9eb",
                    Foreground = "#67C28A",
                    BorderColor = "#dbe4d7",
                    Icon = "\ue63c"
                }
            },
            {
                MessageType.WARNING,
                new MessageTypeData {
                    Background = "#fdf6ec",
                    Foreground = "#E6A23C",
                    BorderColor = "#e8e1d8",
                    Icon = "\ue6d2"
                }
            },
            {
                MessageType.ERROR,
                new MessageTypeData {
                    Background = "#fef0f0",
                    Foreground = "#F66C6C",
                    BorderColor = "#eee1e1",
                    Icon = "\ue6c6"
                }
            }
        };

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
            BoxBackground = MessageMap[MessageType].Background;
            BoxForeground = MessageMap[MessageType].Foreground;
            BorderColor = MessageMap[MessageType].BorderColor;
            Icon = MessageMap[MessageType].Icon;
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

        private class MessageTypeData {
            public string Background { get; set; } = "";
            public string Foreground { get; set; } = "";
            public string BorderColor { get; set; } = "";
            public string Icon { get; set; } = "";
        }

    }

    public enum MessageType {
        INFO, SUCCESS, WARNING, ERROR
    }
}
