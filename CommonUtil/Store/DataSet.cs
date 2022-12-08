using CommonUtil.View;

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

    /// <summary>
    /// 菜单功能
    /// </summary>
    public static readonly IReadOnlyList<ToolMenuItem> ToolMenuItems = new List<ToolMenuItem>() {
        new("Base64 编码/解码", $"{Global.ImagePath}base64.png", typeof(Base64ToolView)),
        new("随机数/文本生成器", $"{Global.ImagePath}random.png", typeof(RandomGeneratorView)),
        new("简体繁体转换", $"{Global.ImagePath}ChineseTransform.png", typeof(ChineseTransformView)),
        new("编码/解码工具", $"{Global.ImagePath}encoding.png", typeof(CommonEncodingView)),
        new("时间戳转换", $"{Global.ImagePath}DateTime.png", typeof(TimeStampView)),
        new("文件分割/合并", $"{Global.ImagePath}Merge.png", typeof(FileMergeSplitView)),
        new("数据散列工具", $"{Global.ImagePath}Encryption.png", typeof(DataDigestView)),
        new("ASCII 表格", $"{Global.ImagePath}ascii.png", typeof(AsciiTableView)),
        new("BMI 计算", $"{Global.ImagePath}bmi.png", typeof(BMICalculatorView)),
        new("进制转换", $"{Global.ImagePath}BaseConversion.png", typeof(BaseConversionView)),
        new("顺序文本生成", $"{Global.ImagePath}order.png", typeof(OrdinalTextGeneratorView)),
        new("文件内容搜索", $"{Global.ImagePath}FileSearch.png", typeof(KeywordFinderView)),
        new("文本工具", $"{Global.ImagePath}TextTool.png", typeof(TextToolView)),
        new("二维码工具", $"{Global.ImagePath}qrcode.png", typeof(QRCodeToolView)),
        new("颜色转换", $"{Global.ImagePath}ColorPicker.png", typeof(ColorTransformView)),
        //new("代码格式化", $"{Global.ImagePath}format.png", typeof(CodeFormatingView)),
        new("成语接龙", $"{Global.ImagePath}IdiomMatching.png", typeof(IdiomMatchingView)),
        new("正则匹配提取", $"{Global.ImagePath}regex.png", typeof(RegexExtractionView)),
        //new("FTP 服务器", $"{Global.ImagePath}ftp.png", typeof(FtpServerView)),
        new("浏览器书签导出", $"{Global.ImagePath}Edge.png", typeof(EdgeBookmarkView)),
        new("代码生成器", $"{Global.ImagePath}CodeGenerator.png", typeof(CodeGeneratorView)),
        new("临时文件版本控制", $"{Global.ImagePath}FileVersion.png", typeof(TempFileVersionControlView)),
        new("简单文件服务器", $"{Global.ImagePath}file-server.png", typeof(SimpleFileSystemServerView)),
        new("代码着色", $"{Global.ImagePath}paint.png", typeof(CodeColorizationView)),
        new("文件下载器", $"{Global.ImagePath}download.png", typeof(DownloaderView)),
        new("集合工具", $"{Global.ImagePath}Intersection.png", typeof(CollectionToolView)),
        new("JSON 数据提取", $"{Global.ImagePath}json.png", typeof(JsonExtractorView)),
    };

    /// <summary>
    /// 时间戳选项
    /// </summary>
    public static readonly IReadOnlyList<string> TimeStampOptions = new List<string>() {
        "毫秒(ms)",
        "秒(s)",
    };

    /// <summary>
    /// 顺序文本类型
    /// </summary>
    public static readonly IReadOnlyDictionary<string, OrdinalTextType> OrdinalTextTypeDict = new Dictionary<string, OrdinalTextType>() {
        {"数字", OrdinalTextType.Number },
        {"字母", OrdinalTextType.Alphabet },
        {"中文数字", OrdinalTextType.ChineseNumber },
        {"中文大写数字", OrdinalTextType.ChineseUpperNumber},
    };

    /// <summary>
    /// 文本去重分割符
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> RemoveDuplicateSplitSymbolDict = new Dictionary<string, string>() {
        { "换行符（⮠  ）", "\n" },
        { "制表符（→）", "\t" },
        { "空格（ ）", " " },
        { "中文逗号（，）", "，" },
        { "英文逗号（,）", "," },
    };

    /// <summary>
    /// 空白处理选项
    /// </summary>
    public static readonly IReadOnlyList<(string, TextTool.TextProcess, TextTool.FileProcess)> WhiteSpaceProcessOptions = new (string, TextTool.TextProcess, TextTool.FileProcess)[] {
        ("去除首尾空白", TextTool.TrimText, TextTool.FileTrimText),
        ("去除每行首部空白", TextTool.TrimLineStart, TextTool.FileTrimLineStart),
        ("去除每行尾部空白", TextTool.TrimLineEnd, TextTool.FileTrimLineEnd),
        ("去除每行首尾空白", TextTool.TrimLine, TextTool.FileTrimLine),
        ("去除空白行", TextTool.RemoveWhiteSpaceLine, TextTool.FileRemoveWhiteSpaceLine),
        ("多个空白字符替换为一个空格", TextTool.ReplaceMultipleWhiteSpaceWithOne, TextTool.FileReplaceMultipleWhiteSpaceWithOne),
    };
}
