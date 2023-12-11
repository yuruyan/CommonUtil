using CommonUtil.View;
using static QRCoder.QRCodeGenerator;

namespace CommonUtil.Store;

/// <summary>
/// 数据集
/// </summary>
public static class DataSet {
    /// <summary>
    /// Application font size options
    /// </summary>
    public static readonly IReadOnlyList<int> ApplicationFontSizeOptions = Enumerable.Range(15, 6).ToList();

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
        new("CommonEncoding", "编码/解码工具", $"{Global.ImageSource}encoding.png", typeof(CommonEncodingView)),
        new("Encryption", "加密/解密工具", $"{Global.ImageSource}Encryption.png", typeof(EncryptionView)),
        new("RandomGenerator", "随机数/文本生成器", $"{Global.ImageSource}random.png", typeof(RandomGeneratorView)),
        new("ChineseTransform", "简体繁体转换", $"{Global.ImageSource}ChineseTransform.png", typeof(ChineseTransformView)),
        new("TimeStamp", "时间戳转换", $"{Global.ImageSource}DateTime.png", typeof(TimeStampView)),
        new("FileMergeSplit", "文件分割/合并", $"{Global.ImageSource}Merge.png", typeof(FileMergeSplitView)),
        new("DataDigest", "数据散列工具", $"{Global.ImageSource}DataDigest.png", typeof(DataDigestView)),
        new("AsciiTable", "ASCII 表格", $"{Global.ImageSource}ascii.png", typeof(AsciiTableView)),
        new("BMICalculator", "BMI 计算", $"{Global.ImageSource}bmi.png", typeof(BMICalculatorView)),
        new("BaseConversion", "进制转换", $"{Global.ImageSource}BaseConversion.png", typeof(BaseConversionView)),
        new("OrdinalTextGenerator", "顺序文本生成", $"{Global.ImageSource}order.png", typeof(OrdinalTextGeneratorView)),
        new("KeywordFinder", "文件内容搜索", $"{Global.ImageSource}FileSearch.png", typeof(KeywordFinderView)),
        new("TextTool", "文本工具", $"{Global.ImageSource}TextTool.png", typeof(TextToolView)),
        new("QRCodeTool", "二维码工具", $"{Global.ImageSource}qrcode.png", typeof(QRCodeToolView)),
        new("ColorTransform", "颜色转换", $"{Global.ImageSource}ColorPicker.png", typeof(ColorTransformView)),
        //new("代码格式化", $"{Global.ImagePath}format.png", typeof(CodeFormatingView)),
        new("IdiomMatching", "成语接龙", $"{Global.ImageSource}IdiomMatching.png", typeof(IdiomMatchingView)),
        new("RegexExtraction", "正则匹配提取", $"{Global.ImageSource}regex.png", typeof(RegexExtractionView)),
        //new("FTP 服务器", $"{Global.ImagePath}ftp.png", typeof(FtpServerView)),
        new("BrowserBookmark", "浏览器书签导出", $"{Global.ImageSource}MicrosoftEdge.png", typeof(BrowserBookmarkView)),
        new("CodeGenerator", "代码生成器", $"{Global.ImageSource}CodeGenerator.png", typeof(CodeGeneratorView)),
        new("TempFileVersionControl", "临时文件版本控制", $"{Global.ImageSource}FileVersion.png", typeof(TempFileVersionControlView)),
        new("SimpleFileSystemServer", "简单文件服务器", $"{Global.ImageSource}file-server.png", typeof(SimpleFileSystemServerView)),
        new("CodeColorization", "代码着色", $"{Global.ImageSource}paint.png", typeof(CodeColorizationView)),
        new("Downloader", "文件下载器", $"{Global.ImageSource}download.png", typeof(DownloaderView)),
        new("CollectionTool", "集合工具", $"{Global.ImageSource}Intersection.png", typeof(CollectionToolView)),
        new("JsonExtractor", "JSON 数据提取", $"{Global.ImageSource}json.png", typeof(JsonExtractorView)),
        new("DesktopAutomation", "桌面自动化", $"{Global.ImageSource}Automation.png", typeof(DesktopAutomationView)),
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
        ("去除首尾空白", WhiteSpaceProcess.TrimText, WhiteSpaceProcess.FileTrimText),
        ("去除每行首部空白", WhiteSpaceProcess.TrimLineStart, WhiteSpaceProcess.FileTrimLineStart),
        ("去除每行尾部空白", WhiteSpaceProcess.TrimLineEnd, WhiteSpaceProcess.FileTrimLineEnd),
        ("去除每行首尾空白", WhiteSpaceProcess.TrimLine, WhiteSpaceProcess.FileTrimLine),
        ("去除空白行", WhiteSpaceProcess.RemoveWhiteSpaceLine, WhiteSpaceProcess.FileRemoveWhiteSpaceLine),
        ("多个空白字符替换为一个空格", WhiteSpaceProcess.ReplaceMultipleWhiteSpaceWithOne, WhiteSpaceProcess.FileReplaceMultipleWhiteSpaceWithOne),
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
        {"大写", (EnglishTextProcess.ToUpperCase, EnglishTextProcess.FileToUpperCase) },
        {"小写", (EnglishTextProcess.ToLowerCase, EnglishTextProcess.FileToLowerCase) },
        {"切换大小写", (EnglishTextProcess.ToggleCase, EnglishTextProcess.FileToggleCase) },
        {"转全角", (EnglishTextProcess.HalfCharToFullChar, EnglishTextProcess.FileHalfCharToFullChar) },
        {"转半角", (EnglishTextProcess.FullCharToHalfChar, EnglishTextProcess.FileFullCharToHalfChar) },
        {"单词首字母大写", (EnglishTextProcess.CapitalizeWords, EnglishTextProcess.FileCapitalizeWords) },
        {"句子首字母大写", (EnglishTextProcess.ToSentenceCase, EnglishTextProcess.FileToSentenceCase) },
    };

    /// <summary>
    /// 进制字符串转换
    /// </summary>
    public static readonly IReadOnlyDictionary<string, (BaseNumberStringConverter.ConvertFromNumber, BaseNumberStringConverter.ConvertToNumber)> BaseNumberConversionOptionDict = new Dictionary<string, (BaseNumberStringConverter.ConvertFromNumber, BaseNumberStringConverter.ConvertToNumber)>() {
        {"8进制", (BaseNumberStringConverter.ConvertFromOctalNumber, BaseNumberStringConverter.ConvertToOctalNumber) },
        {"16进制ASCII", (BaseNumberStringConverter.ConvertFromHexASCIINumber, BaseNumberStringConverter.ConvertToHexASCIINumber) },
        {"16进制2字节Unicode", (BaseNumberStringConverter.ConvertFromHexUnicodeNumber, BaseNumberStringConverter.ConvertToHexUnicodeNumber) },
        {"16进制4字节Unicode", (BaseNumberStringConverter.ConvertFromHexFullUnicodeNumber, BaseNumberStringConverter.ConvertToHexFullUnicodeNumber) },
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
    /// MarkdownTableConversion 列分隔符选项
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> MarkdownTableConversionSplitOptionDict = PrependLineNumberSplitOptionDict.ToDictionary(
        pair => pair.Key,
        pair => pair.Value
    );

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
