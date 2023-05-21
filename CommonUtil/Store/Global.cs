namespace CommonUtil.Store;

public static class Global {
    /// <summary>
    /// 多任务并发数
    /// </summary>
    public const int ConcurrentTaskCount = 8;
    public const string AppTitle = "工具集";
    public const string DataSource = "/CommonUtil.Data;component/Resources";
    public const string ImageSource = $"{DataSource}/Images/";
    /// <summary>
    /// 当前可执行文件目录别名
    /// </summary>
    public static readonly string ApplicationPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase ?? string.Empty;
    private static readonly string _CacheDirectory = Path.Combine(Global.ApplicationPath, "cache");
    /// <summary>
    /// MenuItemsDll 目录，文件名和 <see cref="ToolMenuItem.Id"/> 一致
    /// </summary>
    public static readonly string MenuItemsDllDirectory = Path.Combine(Global.ApplicationPath, "resources/MenuItems");
    /// <summary>
    /// 缓存文件目录
    /// </summary>
    public static string CacheDirectory {
        get {
            // 检查缓存文件目录是否存在，不存在则创建
            if (!Directory.Exists(Global._CacheDirectory)) {
                Directory.CreateDirectory(Global._CacheDirectory);
            }
            return _CacheDirectory;
        }
    }
    /// <summary>
    /// 命令行参数
    /// </summary>
    internal static readonly CommandLineArgument CommandLineArgument = CommandLineArgument.Parse(Environment.GetCommandLineArgs());
    /// <summary>
    /// 菜单项目
    /// </summary>
    internal static readonly IReadOnlyList<ToolMenuItem> MenuItems = DataSet.ToolMenuItems;
}
