using CommonUtil.Core.Model;
using ModernWpf.Controls;

namespace CommonUtil.View;

public partial class AddFtpServerUserDialog : ContentDialog {

    public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(AddFtpServerUserDialog), new PropertyMetadata(""));
    public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(AddFtpServerUserDialog), new PropertyMetadata(""));
    public static readonly DependencyProperty PermissionProperty = DependencyProperty.Register("Permission", typeof(FtpServerUserPermission), typeof(AddFtpServerUserDialog), new PropertyMetadata(FtpServerUserPermission.R));

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username {
        get { return (string)GetValue(UsernameProperty); }
        set { SetValue(UsernameProperty, value); }
    }
    /// <summary>
    /// 密码
    /// </summary>
    public string Password {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }
    /// <summary>
    /// 权限
    /// </summary>
    public FtpServerUserPermission Permission {
        get { return (FtpServerUserPermission)GetValue(PermissionProperty); }
        set { SetValue(PermissionProperty, value); }
    }
    /// <summary>
    /// UserInfo copy
    /// </summary>
    public FtpServerUserInfo UserInfo {
        get {
            return new() {
                Username = Username,
                Password = Password,
                Permission = Permission,
            };
        }
        set {
            Username = value.Username;
            Password = value.Password;
            Permission = value.Permission;
        }
    }

    public AddFtpServerUserDialog() {
        InitializeComponent();
    }

    /// <summary>
    /// permission 改变
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PermissionComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (sender is ComboBox box) {
            if (box.SelectedItem is FrameworkElement element) {
                bool status = Enum.TryParse(element.Tag.ToString(), out FtpServerUserPermission permission);
                if (status) {
                    Permission = permission;
                }
            }
        }
    }
}

