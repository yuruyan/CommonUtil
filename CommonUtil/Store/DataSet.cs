namespace CommonUtil.Store;

/// <summary>
/// 数据集
/// </summary>
public static class DataSet {
    /// <summary>
    /// 代理类型
    /// </summary>
    public static readonly IReadOnlyList<string> ProxyTypes = new List<string>() {
        "http",
        "https",
        "socks4",
        "socks4a",
        "socks5",
    };
}
