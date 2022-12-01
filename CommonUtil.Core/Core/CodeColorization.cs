using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;

namespace CommonUtil.Core;

public static class CodeColorization {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// 代码着色解压缩目录
    /// </summary>
    private const string CodeColorSchemeFolder = "resource/CodeStyle";
    /// <summary>
    /// 代码着色配置文件
    /// </summary>
    private const string CodeColorSchemeConfig = CodeColorSchemeFolder + "/CodeColorScheme.json";
    /// <summary>
    /// 代码着色压缩资源文件
    /// </summary>
    private const string CodeColorSchemeResource = CodeColorSchemeFolder + "/CodeColorScheme.zip";
    /// <summary>
    /// 代码着色配置列表
    /// </summary>
    private static readonly IEnumerable<SchemeInfo> Schemes;
    /// <summary>
    /// 语言名称
    /// </summary>
    public static readonly IEnumerable<string> Languages;

    static CodeColorization() {
        var themes = JsonConvert.DeserializeObject<IEnumerable<SchemeInfo>>(File.ReadAllText(CodeColorSchemeConfig));
        if (themes is null) {
            Logger.Fatal("解析代码颜色配置文件失败");
            throw new JsonSerializationException($"解析代码颜色配置文件失败");
        }
        Schemes = themes;
        Languages = themes.Select(s => s.Name);
        Logger.Debug("解析代码颜色配置文件成功");
        CheckAndDecompressResourceFile();
        RegisterThemes(themes);
        Logger.Debug("注册代码颜色配置文件成功");
    }

    /// <summary>
    /// 检查资源文件是否已解压
    /// </summary>
    private static void CheckAndDecompressResourceFile() {
        // 实际数量小于配置文件数
        if (Directory.GetFiles(CodeColorSchemeFolder).Length - 2 < Schemes.Count()) {
            // 解压文件
            ZipFile.ExtractToDirectory(CodeColorSchemeResource, CodeColorSchemeFolder);
        }
    }

    /// <summary>
    /// 注册主题
    /// </summary>
    /// <param name="themes"></param>
    private static void RegisterThemes(IEnumerable<SchemeInfo> themes) {
        foreach (var theme in themes) {
            using var stream = File.Open(
                Path.Combine(CodeColorSchemeFolder, theme.ResourcePath),
                FileMode.Open,
                FileAccess.Read
            );
            using var reader = new XmlTextReader(stream);
            HighlightingManager.Instance.RegisterHighlighting(
                theme.Name,
                theme.FileTypes,
                HighlightingLoader.Load(reader, HighlightingManager.Instance)
             );
        }
    }

    private struct SchemeInfo {
        public string Name;
        public string[] FileTypes;
        public string ResourcePath;

        public SchemeInfo(string name, string[] fileTypes, string resourcePath) {
            Name = name;
            FileTypes = fileTypes;
            ResourcePath = resourcePath;
        }
    }
}
