using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace CommonUtil.Core;

public static class CodeColorization {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// 代码着色配置列表
    /// </summary>
    private static readonly ICollection<SchemeInfo> Schemes;
    /// <summary>
    /// 语言名称
    /// </summary>
    public static readonly ICollection<string> Languages;

    static CodeColorization() {
        var themes = JsonConvert.DeserializeObject<IList<SchemeInfo>>(
            Encoding.UTF8.GetString(Resource.Resource.CodeColorSchemeConfig)
        );
        if (themes is null) {
            Logger.Fatal("解析代码颜色配置文件失败");
            throw new JsonSerializationException($"解析代码颜色配置文件失败");
        }
        Schemes = themes;
        Languages = themes.Select(s => s.Name).ToList();
        Logger.Debug("解析代码颜色配置文件成功");
        RegisterThemes(themes);
        Logger.Debug("注册代码颜色配置文件成功");
    }

    /// <summary>
    /// 注册主题
    /// </summary>
    /// <param name="themes"></param>
    private static void RegisterThemes(IEnumerable<SchemeInfo> themes) {
        using var codeColorSchemeStream = new MemoryStream(Resource.Resource.CodeColorScheme);
        using var archive = new ZipArchive(codeColorSchemeStream, ZipArchiveMode.Read);
        foreach (var theme in themes) {
            using var entry = archive.GetEntry(theme.ResourceName)!.Open();
            using var reader = new XmlTextReader(entry);
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
        public string ResourceName;

        public SchemeInfo(string name, string[] fileTypes, string resourceName) {
            Name = name;
            FileTypes = fileTypes;
            ResourceName = resourceName;
        }
    }
}
