using CommonUITools.Model;
using CommonUtil.View;
using System.Threading;

namespace CommonUtil.Store;

public struct ToolMenuItem {
    public string Name { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public Type ClassType { get; set; } = default!;

    public ToolMenuItem() { }

    public override string ToString() {
        return $"{{{nameof(Name)}={Name}, {nameof(ImagePath)}={ImagePath}, {nameof(ClassType)}={ClassType}}}";
    }
}

public static class Global {
    /// <summary>
    /// 多任务并发数
    /// </summary>
    public const int ConcurrentTaskCount = 8;
    public const string AppTitle = "工具集";
    public const string ImagePath = "/Resource/image/";
    /// <summary>
    /// 当前可执行文件目录别名
    /// </summary>
    public static readonly string ApplicationPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase ?? string.Empty;
    private static readonly string _CacheDirectory = Path.Combine(Global.ApplicationPath, "cache");
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
    /// 菜单项目
    /// </summary>
    public static readonly List<ToolMenuItem> MenuItems = new() {
        new() { Name = "Base64 编码/解码", ImagePath = ImagePath + "base64.png", ClassType = typeof(Base64ToolView) },
        new() { Name = "随机数/文本生成器", ImagePath = ImagePath + "random.png", ClassType = typeof(RandomGeneratorView) },
        new() { Name = "简体繁体转换", ImagePath = ImagePath + "ChineseTransform.png", ClassType = typeof(ChineseTransformView) },
        new() { Name = "编码/解码工具", ImagePath = ImagePath + "encoding.png", ClassType = typeof(CommonEncodingView) },
        new() { Name = "时间戳转换", ImagePath = ImagePath + "DateTime.png", ClassType = typeof(TimeStampView) },
        new() { Name = "文件分割/合并", ImagePath = ImagePath + "Merge.png", ClassType = typeof(FileMergeSplitView) },
        new() { Name = "数据散列工具", ImagePath = ImagePath + "Encryption.png", ClassType = typeof(DataDigestView) },
        new() { Name = "ASCII 表格", ImagePath = ImagePath + "ascii.png", ClassType = typeof(AsciiTableView) },
        new() { Name = "BMI 计算", ImagePath = ImagePath + "bmi.png", ClassType = typeof(BMICalculatorView) },
        new() { Name = "进制转换", ImagePath = ImagePath + "BaseConversion.png", ClassType = typeof(BaseConversionView) },
        new() { Name = "顺序文本生成", ImagePath = ImagePath + "order.png", ClassType = typeof(OrdinalTextGeneratorView) },
        new() { Name = "文件内容搜索", ImagePath = ImagePath + "FileSearch.png", ClassType = typeof(KeywordFinderView) },
        new() { Name = "文本工具", ImagePath = ImagePath + "TextTool.png", ClassType = typeof(TextToolView) },
        new() { Name = "二维码工具", ImagePath = ImagePath + "qrcode.png", ClassType = typeof(QRCodeToolView) },
        new() { Name = "颜色转换", ImagePath = ImagePath + "ColorPicker.png", ClassType = typeof(ColorTransformView) },
        //new() { Name = "代码格式化", ImagePath = ImagePath + "format.png", ClassType = typeof(CodeFormatingView) },
        new() { Name = "成语接龙", ImagePath = ImagePath + "IdiomMatching.png", ClassType = typeof(IdiomMatchingView) },
        new() { Name = "正则匹配提取", ImagePath = ImagePath + "regex.png", ClassType = typeof(RegexExtractionView) },
        //new() { Name = "FTP 服务器", ImagePath = ImagePath + "ftp.png", ClassType = typeof(FtpServerView) },
        new() { Name = "浏览器书签导出", ImagePath = ImagePath + "Edge.png", ClassType = typeof(EdgeBookmarkView) },
        new() { Name = "代码生成器", ImagePath = ImagePath + "CodeGenerator.png", ClassType = typeof(CodeGeneratorView) },
        new() { Name = "临时文件版本控制", ImagePath = ImagePath + "FileVersion.png", ClassType = typeof(TempFileVersionControlView) },
        new() { Name = "简单文件服务器", ImagePath = ImagePath + "file-server.png", ClassType = typeof(SimpleFileSystemServerView) },
        new() { Name = "代码着色", ImagePath = ImagePath + "paint.png", ClassType = typeof(CodeColorizationView) },
        new() { Name = "文件下载器", ImagePath = ImagePath + "download.png", ClassType = typeof(DownloaderView) },
        new() { Name = "集合工具", ImagePath = ImagePath + "Intersection.png", ClassType = typeof(CollectionToolView) },
        new() { Name = "JSON 数据提取", ImagePath = ImagePath + "json.png", ClassType = typeof(JsonExtractorView) },
    };
}

public static class GlobalUtils {
    /// <summary>
    /// 在任务完成后时，更新 ProcessStatus Status，如果是 Successful 则同时设置 Process 为 1
    /// </summary>
    /// <param name="status"></param>
    /// <param name="result"></param>
    /// <remarks>可在任意线程调用</remarks>
    public static void UpdateProcessStatusWhenCompleted(FileProcessStatus status, ProcessResult result) {
        App.Current.Dispatcher.Invoke(() => {
            status.Status = result;
            if (result == ProcessResult.Successful) {
                status.Process = 1;
            }
        });
    }

    /// <summary>
    /// 在任务完成后时，更新 ProcessStatus，如果任务没有取消则设置 Status = Successful,
    /// Process = 1, FileSize = fileSize，否则设置 Status = Interrupted
    /// </summary>
    /// <param name="status"></param>
    /// <param name="token"></param>
    /// <param name="fileSize"></param>
    /// <remarks>可在任意线程调用</remarks>
    public static void UpdateProcessStatusWhenCompleted(FileProcessStatus status, CancellationToken token, long fileSize) {
        App.Current.Dispatcher.Invoke(() => {
            if (token.IsCancellationRequested) {
                status.Status = ProcessResult.Interrupted;
            } else {
                status.Status = ProcessResult.Successful;
                status.Process = 1;
                status.FileSize = fileSize;
            }
        });
    }

    /// <summary>
    /// 在任务完成后时，更新 ProcessStatus，如果任务没有取消则设置 Status = Successful, 
    /// Process = 1，否则设置 Status = Interrupted
    /// </summary>
    /// <param name="status"></param>
    /// <param name="token"></param>
    public static void UpdateProcessStatusWhenCompleted(FileProcessStatus status, CancellationToken token) {
        App.Current.Dispatcher.Invoke(() => {
            if (token.IsCancellationRequested) {
                status.Status = ProcessResult.Interrupted;
            } else {
                status.Status = ProcessResult.Successful;
                status.Process = 1;
            }
        });
    }

    /// <summary>
    /// 更新 ProcessStatus Process
    /// </summary>
    /// <param name="status"></param>
    /// <param name="process"></param>
    /// <remarks>可在任意线程调用</remarks>
    public static void UpdateProcessStatus(FileProcessStatus status, double process) {
        App.Current.Dispatcher.Invoke(() => status.Process = process);
    }
}