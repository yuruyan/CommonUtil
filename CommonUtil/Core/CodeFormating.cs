using CommonUtil.Store;
using Flurl.Http;
using System.Diagnostics;

namespace CommonUtil.Core;

public class CodeFormating {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly string UncrustifyDirectory = "resource/lib/uncrustify";

    // C, C++, C#, ObjectiveC, D, Java, Pawn and VALA
    //Angular, Css, Graphql, Html, Js, Json, Json5, Less, Markdown, Mdx, Scss, Typescript, Vue, Yaml,
    public enum Lang {
        Angular,
        Assembly,
        C,
        CPlusPlus,
        CSharp,
        Css,
        D,
        Dart,
        Delphi,
        Fortran,
        Go,
        Graphql,
        Groovy,
        Html,
        Java,
        Js,
        Json,
        Json5,
        Julia,
        Kotlin,
        Less,
        Lisp,
        Lua,
        Markdown,
        MATLAB,
        Mdx,
        ObjectiveC,
        Perl,
        PHP,
        Prolog,
        Python,
        R,
        Ruby,
        Rust,
        Scala,
        Scss,
        SQL,
        Swift,
        Typescript,
        VisualBasic,
        Vue,
        Yaml,
    }

    public static readonly Dictionary<string, Lang> LanguageDict = new() {
    { "Angular", Lang.Angular },
    { "Assembly", Lang.Assembly },
    { "C", Lang.C },
    { "C#", Lang.CSharp },
    { "C++", Lang.CPlusPlus },
    { "Css", Lang.Css },
    { "D", Lang.D },
    { "Dart", Lang.Dart },
    { "Delphi", Lang.Delphi },
    { "Fortran", Lang.Fortran },
    { "Go", Lang.Go },
    { "Graphql", Lang.Graphql },
    { "Groovy", Lang.Groovy },
    { "Html", Lang.Html },
    { "Java", Lang.Java },
    { "JavaScript", Lang.Js },
    { "Json", Lang.Json },
    { "Json5", Lang.Json5 },
    { "Julia", Lang.Julia },
    { "Kotlin", Lang.Kotlin },
    { "Less", Lang.Less },
    { "Lisp", Lang.Lisp },
    { "Lua", Lang.Lua },
    { "Markdown", Lang.Markdown },
    { "MATLAB", Lang.MATLAB },
    { "Mdx", Lang.Mdx },
    { "Objective-c", Lang.ObjectiveC },
    { "Perl", Lang.Perl },
    { "PHP", Lang.PHP },
    { "Prolog", Lang.Prolog },
    { "Python", Lang.Python },
    { "R", Lang.R },
    { "Ruby", Lang.Ruby },
    { "Rust", Lang.Rust },
    { "Scala", Lang.Scala },
    { "Scss", Lang.Scss },
    { "SQL", Lang.SQL },
    { "Swift", Lang.Swift },
    { "Typescript", Lang.Typescript },
    { "VisualBasic", Lang.VisualBasic },
    { "Vue", Lang.Vue },
    { "Yaml", Lang.Yaml },
};

    public static readonly Dictionary<Lang, string> LangExtensionDict = new() {
    { Lang.Angular, ".ts" },
    { Lang.Assembly, ".asm" },
    { Lang.C, ".c" },
    { Lang.CPlusPlus, ".cpp" },
    { Lang.CSharp, ".cs" },
    { Lang.Css, ".css" },
    { Lang.D, ".d" },
    { Lang.Dart, ".dart" },
    { Lang.Delphi, "" },
    { Lang.Fortran, "" },
    { Lang.Go, ".go" },
    { Lang.Graphql, "" },
    { Lang.Groovy, ".groovy" },
    { Lang.Html, "" },
    { Lang.Java, ".java" },
    { Lang.Js, ".js" },
    { Lang.Json, ".json" },
    { Lang.Json5, "" },
    { Lang.Julia, "" },
    { Lang.Kotlin, ".kt" },
    { Lang.Less, ".less" },
    { Lang.Lisp, ".lisp" },
    { Lang.Lua, ".lua" },
    { Lang.Markdown, ".md" },
    { Lang.MATLAB, "" },
    { Lang.Mdx, "" },
    { Lang.ObjectiveC, ".m" },
    { Lang.Perl, "" },
    { Lang.PHP, ".php" },
    { Lang.Prolog, "" },
    { Lang.Python, ".py" },
    { Lang.R, "" },
    { Lang.Ruby, "" },
    { Lang.Rust, ".rs" },
    { Lang.Scala, "" },
    { Lang.Scss, ".scss" },
    { Lang.SQL, ".sql" },
    { Lang.Swift, "" },
    { Lang.Typescript, ".ts" },
    { Lang.VisualBasic, ".vb" },
    { Lang.Vue, ".vue" },
    { Lang.Yaml, ".yaml" },
};

    /// <summary>
    /// 格式化代码
    /// </summary>
    /// <param name="code"></param>
    /// <param name="lang"></param>
    /// <returns></returns>
    public static async Task<string> FormatAsync(string code, Lang lang) {
        switch (lang) {
            case Lang.C:
            case Lang.CPlusPlus:
            case Lang.CSharp:
            case Lang.ObjectiveC:
            case Lang.D:
            case Lang.Java:
                return Format1(code, lang);
            case Lang.Angular:
            case Lang.Css:
            case Lang.Graphql:
            case Lang.Html:
            case Lang.Js:
            case Lang.Json:
            case Lang.Json5:
            case Lang.Less:
            case Lang.Markdown:
            case Lang.Mdx:
            case Lang.Scss:
            case Lang.Typescript:
            case Lang.Vue:
            case Lang.Yaml:
                return await Format2Async(code, lang);
            //case Lang.Assembly:
            //    break;
            //case Lang.Dart:
            //    break;
            //case Lang.Delphi:
            //    break;
            //case Lang.Fortran:
            //    break;
            //case Lang.Go:
            //    break;
            //case Lang.Groovy:
            //    break;
            //case Lang.Julia:
            //    break;
            //case Lang.Kotlin:
            //    break;
            //case Lang.Lisp:
            //    break;
            //case Lang.Lua:
            //    break;
            //case Lang.MATLAB:
            //    break;
            //case Lang.Perl:
            //    break;
            //case Lang.PHP:
            //    break;
            //case Lang.Prolog:
            //    break;
            //case Lang.Python:
            //    break;
            //case Lang.R:
            //    break;
            //case Lang.Ruby:
            //    break;
            //case Lang.Rust:
            //    break;
            //case Lang.Scala:
            //    break;
            //case Lang.SQL:
            //    break;
            //case Lang.Swift:
            //    break;
            //case Lang.VisualBasic:
            //    break;
            default:
                return code;
        }
    }

    /// <summary>
    /// 格式化方式 1，采用 uncrustify 实现
    /// </summary>
    /// <param name="code"></param>
    /// <param name="lang"></param>
    /// <returns></returns>
    private static string Format1(string code, Lang lang) {
        string filename = $"{GenerateCacheFileName()}{LangExtensionDict[lang]}";
        string libDir = Path.Combine(Global.ApplicationPath, UncrustifyDirectory);
        string cacheFile = Path.Combine(Global.CacheDirectory, filename);
        string configFile = Path.Combine(libDir, "defaults.cfg");
        string cacheOutFile = Path.Combine(Global.CacheDirectory, "out-" + filename);
        // 写入文件
        File.WriteAllText(cacheFile, code);
        var proc = new Process();
        proc.StartInfo.FileName = Path.Combine(libDir, "uncrustify.exe");
        proc.StartInfo.Arguments = $"-c \"{configFile}\" -f \"{cacheFile}\" -o \"{cacheOutFile}\"";
        if (Config.Environment == Model.ApplicationEnvironment.Development) {
            proc.StartInfo.RedirectStandardOutput = true;
        }
        proc.Start();
        proc.WaitForExit(); // 等待完成
        return File.ReadAllText(cacheOutFile);
    }

    private static readonly Random Random = new();

    /// <summary>
    /// 生成缓存文件名
    /// </summary>
    /// <returns></returns>
    private static string GenerateCacheFileName() {
        return $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}-{Random.NextDouble().ToString().Replace(".", "")}";
    }

    /// <summary>
    /// 格式化方式 2，采用 prettier 实现
    /// </summary>
    /// <param name="code"></param>
    /// <param name="lang"></param>
    /// <returns></returns>
    private static async Task<string> Format2Async(string code, Lang lang) {
        Server.CheckNodeJsServer();
        var resp = await $"{Server.NodeJsServerBaseUrl}/codeformating"
                     .PostJsonAsync(new {
                         code,
                         lang = lang.ToString(),
                     });
        var data = await resp.GetJsonAsync<JsonResponse<string>>();
        if (data.Code != 200) {
            Logger.Info("CodeFormat failed, message: " + data.Message);
            return code;
        }
        return CommonUtils.NullCheck(data.Data);
    }
}
