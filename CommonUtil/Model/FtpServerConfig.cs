using Newtonsoft.Json;
using System.Collections.Generic;

namespace CommonUtil.Model;

public enum FtpServerUserPermission {
    /// <summary>
    /// 只读
    /// </summary>
    R,
    /// <summary>
    /// 读写
    /// </summary>
    W
}

public class FtpServerUserInfo {
    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;
    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;
    /// <summary>
    /// 权限
    /// </summary>
    [JsonProperty("permission")]
    public FtpServerUserPermission Permission { get; set; } = FtpServerUserPermission.R;

    public override string ToString() {
        return $"{{{nameof(Username)}={Username}, {nameof(Password)}={Password}, {nameof(Permission)}={Permission.ToString()}}}";
    }
}

public class FtpServerConfig {
    /// <summary>
    /// 根目录
    /// </summary>
    [JsonProperty("root")]
    public string Root { get; set; } = string.Empty;
    /// <summary>
    /// 是否支持匿名登录
    /// </summary>
    [JsonProperty("anonymous")]
    public bool Anonymous { get; set; } = false;
    /// <summary>
    /// 用户列表
    /// </summary>
    [JsonProperty("userlist")]
    public List<FtpServerUserInfo> UserList { get; set; }
}
