using CommonUtil.View;
using static QRCoder.QRCodeGenerator;

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
        new("桌面自动化", $"{Global.ImagePath}Automation.png", typeof(DesktopAutomation)),
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

    /// <summary>
    /// 摘要算法选项
    /// </summary>
    public static readonly IReadOnlyList<(string, DataDigest.TextDigest, DataDigest.StreamDigest)> DigestOptions = new (string, DataDigest.TextDigest, DataDigest.StreamDigest)[] {
        ("MD2", DataDigest.MD2Digest, DataDigest.MD2Digest),
        ("MD4", DataDigest.MD4Digest, DataDigest.MD4Digest),
        ("MD5", DataDigest.MD5Digest, DataDigest.MD5Digest),
        ("SHA1", DataDigest.SHA1Digest, DataDigest.SHA1Digest),
        ("SHA3", DataDigest.SHA3Digest, DataDigest.SHA3Digest),
        ("SHA224", DataDigest.SHA224Digest, DataDigest.SHA224Digest),
        ("SHA256", DataDigest.SHA256Digest, DataDigest.SHA256Digest),
        ("SHA384", DataDigest.SHA384Digest, DataDigest.SHA384Digest),
        ("SHA512", DataDigest.SHA512Digest, DataDigest.SHA512Digest),
        ("WhirlpoolDigest", DataDigest.WhirlpoolDigest, DataDigest.WhirlpoolDigest),
        ("TigerDigest", DataDigest.TigerDigest, DataDigest.TigerDigest),
        ("SM3Digest", DataDigest.SM3Digest, DataDigest.SM3Digest),
        ("ShakeDigest", DataDigest.ShakeDigest, DataDigest.ShakeDigest),
        ("RipeMD128Digest", DataDigest.RipeMD128Digest, DataDigest.RipeMD128Digest),
        ("RipeMD160Digest", DataDigest.RipeMD160Digest, DataDigest.RipeMD160Digest),
        ("RipeMD256Digest", DataDigest.RipeMD256Digest, DataDigest.RipeMD256Digest),
        ("RipeMD320Digest", DataDigest.RipeMD320Digest, DataDigest.RipeMD320Digest),
        ("KeccakDigest", DataDigest.KeccakDigest, DataDigest.KeccakDigest),
        ("Gost3411Digest", DataDigest.Gost3411Digest, DataDigest.Gost3411Digest),
        ("Gost3411_2012_256Digest", DataDigest.Gost3411_2012_256Digest, DataDigest.Gost3411_2012_256Digest),
        ("Gost3411_2012_512Digest", DataDigest.Gost3411_2012_512Digest, DataDigest.Gost3411_2012_512Digest),
        ("Blake2bDigest", DataDigest.Blake2bDigest, DataDigest.Blake2bDigest),
        ("Blake2sDigest", DataDigest.Blake2sDigest, DataDigest.Blake2sDigest),
    };

    /// <summary>
    /// 英文文本处理选项
    /// </summary>
    public static readonly IReadOnlyDictionary<string, (TextTool.TextProcess, TextTool.FileProcess)> EnglishTextProcessOptionDict = new Dictionary<string, (TextTool.TextProcess, TextTool.FileProcess)>() {
        {"大写", (TextTool.ToUpperCase, TextTool.FileToUpperCase) },
        {"小写", (TextTool.ToLowerCase, TextTool.FileToLowerCase) },
        {"切换大小写", (TextTool.ToggleCase, TextTool.FileToggleCase) },
        {"转全角", (TextTool.HalfCharToFullChar, TextTool.FileHalfCharToFullChar) },
        {"转半角", (TextTool.FullCharToHalfChar, TextTool.FileFullCharToHalfChar) },
        {"单词首字母大写", (TextTool.CapitalizeWords, TextTool.FileCapitalizeWords) },
        {"句子首字母大写", (TextTool.ToSentenceCase, TextTool.FileToSentenceCase) },
    };

    /// <summary>
    /// 增加行号分割文本选项
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> PrependLineNumberSplitOptionDict = new Dictionary<string, string>() {
        { "制表符（→）", "\t" },
        { "空格（ ）", " " },
        { "中文逗号（，）", "，" },
        { "英文逗号（,）", "," },
        { "中文句号（。）", "。" },
        { "英文句号（.）", "." },
    };

    /// <summary>
    /// 翻转模式选项
    /// </summary>
    public static readonly IReadOnlyDictionary<string, InversionMode> InversionModeDict = new Dictionary<string, InversionMode>() {
        {"水平翻转", InversionMode.Horizontal },
        {"垂直翻转", InversionMode.Vertical},
        {"全部翻转", InversionMode.Both },
    };

    /// <summary>
    /// 二维码容错率
    /// </summary>
    public static readonly IReadOnlyDictionary<byte, ECCLevel> QRCodeECCLevelDict = new Dictionary<byte, ECCLevel>() {
        {7, ECCLevel.L },
        {15, ECCLevel.M },
        {25, ECCLevel.Q },
        {35, ECCLevel.H },
    };

    /// <summary>
    /// 二维码图片质量
    /// </summary>
    public static readonly IReadOnlyDictionary<string, byte> QRCodeImageQualityDict = new Dictionary<string, byte>() {
        {"低", 8},
        {"中", 16},
        {"高", 32 }
    };
}
