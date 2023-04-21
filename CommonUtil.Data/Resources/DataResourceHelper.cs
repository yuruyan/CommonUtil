using CommonUtil.Data.Model;
using Newtonsoft.Json;
using System.Text;

namespace CommonUtil.Data.Resources;

public static partial class DataResourceHelper {
    private static readonly Lazy<IReadOnlyList<AsciiInfo>> _AsciiInfoList = new(() => {
        return JsonConvert.DeserializeObject<List<AsciiInfo>>(
            Encoding.UTF8.GetString(DataResource.Ascii)
        )!;
    });
    private static readonly Lazy<IReadOnlyDictionary<char, char>> _ChineseTraditionalSimplifiedCharacterDict = new(() => {
        return JsonConvert.DeserializeObject<Dictionary<char, char>>(
            Encoding.UTF8.GetString(DataResource.ChineseCharacterMap)
        )!;
    });
    private static readonly Lazy<IReadOnlyList<CodeColorizationSchemeInfo>> _CodeColorizationSchemeInfoList = new(() => {
        return JsonConvert.DeserializeObject<List<CodeColorizationSchemeInfo>>(
                Encoding.UTF8.GetString(DataResource.CodeColorSchemeConfig)
        )!;
    });
    private static readonly Lazy<byte[]> _CodeColorizationColorSchemeDark = new(() => {
        return DataResource.CodeColorSchemeDark;
    });
    private static readonly Lazy<byte[]> _CodeColorizationColorSchemeLight = new(() => {
        return DataResource.CodeColorSchemeLight;
    });
    private static readonly Lazy<IReadOnlyList<KeyValuePair<string, string>>> _CommonRegexList = new(() => {
        return JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(
            Encoding.UTF8.GetString(DataResource.CommonRegex)
        )!;
    });
    private static readonly Lazy<IReadOnlyList<string>> _ChineseIdioms = new(() => {
        return DataResource.Idioms
            .Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
    });
    private static readonly Lazy<IReadOnlyDictionary<string, string>> _ProgramingGetTimeStampList = new(() => {
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(
            Encoding.UTF8.GetString(DataResource.ProgramingGetTimeStamp)
        )!;
    });
}

public static partial class DataResourceHelper {
    public static IReadOnlyList<AsciiInfo> AsciiInfoList => _AsciiInfoList.Value;
    public static IReadOnlyDictionary<char, char> ChineseTraditionalSimplifiedCharacterDict => _ChineseTraditionalSimplifiedCharacterDict.Value;
    public static IReadOnlyList<CodeColorizationSchemeInfo> CodeColorizationSchemeInfoList => _CodeColorizationSchemeInfoList.Value;
    public static byte[] CodeColorizationColorSchemeDark => _CodeColorizationColorSchemeDark.Value;
    public static byte[] CodeColorizationColorSchemeLight => _CodeColorizationColorSchemeLight.Value;
    public static IReadOnlyList<KeyValuePair<string, string>> CommonRegexList => _CommonRegexList.Value;
    public static IReadOnlyList<string> ChineseIdioms => _ChineseIdioms.Value;
    public static IReadOnlyDictionary<string, string> ProgramingGetTimeStampList => _ProgramingGetTimeStampList.Value;
}
