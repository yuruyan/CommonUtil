using CommonUtil.Core;
using CommonUtil.Model;
using CommonUtil.Store;
using CommonUtil.Utils;
using ModernWpf.Controls;
using NLog;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CommonUtil.View {
    public partial class FtpServerView : System.Windows.Controls.Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty RootDirectoryProperty = DependencyProperty.Register("RootDirectory", typeof(string), typeof(FtpServerView), new PropertyMetadata(""));
        public static readonly DependencyProperty UserInfoListProperty = DependencyProperty.Register("UserInfoList", typeof(ObservableCollection<FtpServerUserInfo>), typeof(FtpServerView), new PropertyMetadata(new ObservableCollection<FtpServerUserInfo>()));
        public static readonly DependencyProperty IsAnonymousProperty = DependencyProperty.Register("IsAnonymous", typeof(bool), typeof(FtpServerView), new PropertyMetadata(false));
        private static readonly DependencyProperty IsStopServerButtonVisibleProperty = DependencyProperty.Register("IsStopServerButtonVisible", typeof(bool), typeof(FtpServerView), new PropertyMetadata(false));

        /// <summary>
        /// 共享根目录
        /// </summary>
        public string RootDirectory {
            get { return (string)GetValue(RootDirectoryProperty); }
            set { SetValue(RootDirectoryProperty, value); }
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        public ObservableCollection<FtpServerUserInfo> UserInfoList {
            get { return (ObservableCollection<FtpServerUserInfo>)GetValue(UserInfoListProperty); }
            set { SetValue(UserInfoListProperty, value); }
        }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnonymous {
            get { return (bool)GetValue(IsAnonymousProperty); }
            set { SetValue(IsAnonymousProperty, value); }
        }
        /// <summary>
        /// 停止按钮是否可见
        /// </summary>
        private bool IsStopServerButtonVisible {
            get { return (bool)GetValue(IsStopServerButtonVisibleProperty); }
            set { SetValue(IsStopServerButtonVisibleProperty, value); }
        }
        /// <summary>
        /// 添加用户 Dialog
        /// </summary>
        private AddFtpServerUserDialog AddFtpServerUserDialog;

        public FtpServerView() {
            UserInfoList.Add(new() { Username = "luxiu", Password = "123", Permission = FtpServerUserPermission.R });
            InitializeComponent();
            // 启动 nodejs 服务
            Task.Run(() => CommonUtils.Try(() => Server.CheckNodeJsServer()));
        }

        /// <summary>
        /// 打开根目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenRootDirectoryMouseUp(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
            try {
                Process.Start("explorer.exe", RootDirectory);
            } catch (Exception error) {
                Widget.MessageBox.Error("打开失败," + error.Message);
                Logger.Info(error);
            }
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddUserClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            if (AddFtpServerUserDialog == null) {
                AddFtpServerUserDialog = new();
            }
            var result = await AddFtpServerUserDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) {
                UserInfoList.Add(AddFtpServerUserDialog.UserInfo);
            }
        }

        /// <summary>
        /// 选择根目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectRootDirectoryClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            var dialog = new VistaFolderBrowserDialog {
                Description = "选择分享目录",
                UseDescriptionForTitle = true
            };
            if (dialog.ShowDialog(Application.Current.MainWindow) == true) {
                RootDirectory = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 修改 userinfo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyUserInfoMouseUp(object sender, MouseButtonEventArgs e) {

        }

        /// <summary>
        /// 删除 user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteUserInfoMouseUp(object sender, MouseButtonEventArgs e) {

        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartServerClick(object sender, RoutedEventArgs e) {
            if (!CheckInput()) {
                return;
            }
            var config = new FtpServerConfig() {
                Root = RootDirectory,
                Anonymous = IsAnonymous,
                UserList = new(UserInfoList)
            };
            Task.Run(async () => {
                bool status = await FtpServer.StartFtpServerAsync(config);
                if (status) {
                    Dispatcher.Invoke(() => IsStopServerButtonVisible = true);
                    Widget.MessageBox.Success("启动成功");
                } else {
                    Widget.MessageBox.Error("启动失败");
                }
            });
        }

        /// <summary>
        /// 检查输入
        /// </summary>
        /// <returns></returns>
        private bool CheckInput() {
            if (string.IsNullOrEmpty(RootDirectory)) {
                Widget.MessageBox.Info("请选择共享目录");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopServerClick(object sender, RoutedEventArgs e) {
            Task.Run(async () => {
                bool status = await FtpServer.StopFtpServerAsync();
                if (status) {
                    Dispatcher.Invoke(() => IsStopServerButtonVisible = false);
                    Widget.MessageBox.Success("停止成功");
                } else {
                    Widget.MessageBox.Error("停止失败");
                }
            });
        }
    }

    public class UserPermissionConverter : IValueConverter {
        private static readonly Dictionary<FtpServerUserPermission, string> UserPermissionCommentDict = new() {
            { FtpServerUserPermission.R, "只读" },
            { FtpServerUserPermission.W, "读写" },
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return UserPermissionCommentDict[(FtpServerUserPermission)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
