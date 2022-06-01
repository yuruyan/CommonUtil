using System;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CommonUtil.Widget {
    public partial class NotificationBox : UserControl {
        private static readonly DependencyProperty BoxForegroundProperty = DependencyProperty.Register("BoxForeground", typeof(string), typeof(NotificationBox), new PropertyMetadata("White"));
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(NotificationBox), new PropertyMetadata(""));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(NotificationBox), new PropertyMetadata(""));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(NotificationBox), new PropertyMetadata(""));
        public static readonly DependencyProperty ClickCallbackProperty = DependencyProperty.Register("ClickCallback", typeof(Action), typeof(NotificationBox), new PropertyMetadata());

        /// <summary>
        /// Background
        /// </summary>
        private string BoxForeground {
            get { return (string)GetValue(BoxForegroundProperty); }
            set { SetValue(BoxForegroundProperty, value); }
        }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        /// <summary>
        /// 显示时间 (ms)
        /// </summary>
        public int ShowingDuration { get; set; } = 4500;
        /// <summary>
        /// 用于添加 MessageBox
        /// </summary>
        public static UIElementCollection? PanelChildren;
        /// <summary>
        /// 关闭通知定时器
        /// </summary>
        private readonly Timer UnLoadTimer;

        /// <summary>
        /// 点击回调
        /// </summary>
        public Action ClickCallback {
            get { return (Action)GetValue(ClickCallbackProperty); }
            set { SetValue(ClickCallbackProperty, value); }
        }

        public static void ShowNotification(string title, string message, MessageType messageType = MessageType.INFO, Action? callback = null) {
            // 检查权限
            if (App.Current.Dispatcher.CheckAccess()) {
                PanelChildren?.Add(new NotificationBox(title, message, messageType, callback));
            } else {
                App.Current.Dispatcher.Invoke(() => {
                    PanelChildren?.Add(new NotificationBox(title, message, messageType, callback));
                });
            }
        }

        public static void Info(string title, string message, Action? callback = null) {
            ShowNotification(title, message, MessageType.INFO, callback);
        }

        public static void Warning(string title, string message, Action? callback = null) {
            ShowNotification(title, message, MessageType.WARNING, callback);
        }

        public static void Success(string title, string message, Action? callback = null) {
            ShowNotification(title, message, MessageType.SUCCESS, callback);
        }

        public static void Error(string title, string message, Action? callback = null) {
            ShowNotification(title, message, MessageType.ERROR, callback);
        }

        public NotificationBox(string title, string message, MessageType messageType = MessageType.INFO, Action? callback = null) {
            Message = message;
            Title = title;
            ClickCallback = callback;
            Icon = WidgetGlobal.MessageInfoDict[messageType].Icon;
            BoxForeground = WidgetGlobal.MessageInfoDict[messageType].Foreground;
            InitializeComponent();
            UnLoadTimer = new Timer(ShowingDuration) { AutoReset = false };
            UnLoadTimer.Elapsed += RootUnLoad;
            UnLoadTimer.Start();
        }

        /// <summary>
        /// 消失时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootUnLoad(object? sender, ElapsedEventArgs e) {
            Dispatcher.Invoke(() => UnLoadNotification());
            UnLoadTimer.Dispose();
        }

        /// <summary>
        /// 关闭通知
        /// </summary>
        private void UnLoadNotification() {
            if (Resources["UnLoadStoryboard"] is Storyboard storyboard) {
                var enumerable = from a in storyboard.Children
                                 where a.Name == "UnLoadHeightAnimation"
                                 select a;
                if (enumerable.Any()) {
                    if (enumerable.First() is DoubleAnimation heightAnimation) {
                        heightAnimation.From = ActualHeight;
                    }
                }
                storyboard.Completed += (s, e) => {
                    Visibility = Visibility.Collapsed;
                    PanelChildren?.Remove(this);
                };
                storyboard.Begin();
            }
        }

        /// <summary>
        /// 显示时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootLoaded(object sender, RoutedEventArgs e) {
            if (Parent is FrameworkElement parent) {
                if (Resources["LoadStoryboard"] is Storyboard storyboard) {
                    var enumerable = from a in storyboard.Children
                                     where a.Name == "LoadHeightAnimation"
                                     select a;
                    if (enumerable.Any()) {
                        if (enumerable.First() is DoubleAnimation heightAnimation) {
                            heightAnimation.To = ActualHeight;
                        }
                    }
                    storyboard.Begin();
                }
            }
        }

        /// <summary>
        /// 关闭通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseMouseUp(object sender, MouseButtonEventArgs e) {
            UnLoadTimer.Stop();
            UnLoadTimer.Dispose();
            UnLoadNotification();
        }

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActionTextBlockMouseUp(object sender, MouseButtonEventArgs e) {
            ClickCallback?.Invoke();
        }
    }
}
