using CommonUITools.Model;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Xml;

namespace CommonUtil.Core;

public static class CodeColorization {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// 代码着色配置列表
    /// </summary>
    private static readonly IReadOnlyList<SchemeInfo> Schemes;
    /// <summary>
    /// 语言名称
    /// </summary>
    public static readonly IReadOnlyList<string> Languages;

    static CodeColorization() {
        var themes = JsonConvert.DeserializeObject<List<SchemeInfo>>(
            Encoding.UTF8.GetString(Resource.Resource.CodeColorSchemeConfig)
        );
        if (themes is null) {
            Logger.Fatal("解析代码颜色配置文件失败");
            throw new JsonSerializationException($"解析代码颜色配置文件失败");
        }
        Schemes = themes;
        Languages = themes.Select(s => s.Name).ToList();
        RegisterThemes(Resource.Resource.CodeColorSchemeLight, ThemeMode.Light);
        RegisterThemes(Resource.Resource.CodeColorSchemeDark, ThemeMode.Dark);
        Logger.Debug("注册代码颜色配置文件成功");
    }

    /// <summary>
    /// 注册主题
    /// </summary>
    /// <param name="colorSchemeResource">资源文件</param>
    /// <param name="themeMode">主题色</param>
    private static void RegisterThemes(byte[] colorSchemeResource, ThemeMode themeMode) {
        using var codeColorSchemeStream = new MemoryStream(colorSchemeResource);
        using var archive = new ZipArchive(codeColorSchemeStream, ZipArchiveMode.Read);
        foreach (var theme in Schemes) {
            using var entry = archive.GetEntry(theme.ResourceName)!.Open();
            using var reader = new XmlTextReader(entry);
            HighlightingManager.Instance.RegisterHighlighting(
                $"{theme.Name}-{themeMode}",
                theme.FileTypes,
                HighlightingLoader.Load(reader, HighlightingManager.Instance)
            );
        }
    }

    /// <summary>
    /// 获取语言对应的 HighlightingDefinition
    /// </summary>
    /// <param name="lang"></param>
    /// <param name="themeMode"></param>
    /// <returns></returns>
    public static IHighlightingDefinition GetHighlighting(string lang, ThemeMode themeMode) {
        return HighlightingManager.Instance.GetDefinition($"{lang}-{themeMode}");
    }

    private struct SchemeInfo {
        public string Name;
        public string[] FileTypes;
        public string ResourceName;

        public SchemeInfo(string name, string[] fileTypes, string resourceName) {
            Name = name;
            FileTypes = fileTypes;
            ResourceName = resourceName;
        }
    }
}
