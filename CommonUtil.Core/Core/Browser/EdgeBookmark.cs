using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.XSSF.UserModel;
using System.Reflection;

namespace CommonUtil.Core;

public class EdgeBookmark {

    private static readonly IMapper mapper;

    static EdgeBookmark() {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<Bookmark, Bookmark>());
        mapper = config.CreateMapper();
    }

    private class Bookmark {
        [JsonProperty("date_added")]
        public string DateAdded { get; set; } = string.Empty;
        public string Guid { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonProperty("show_icon")]
        public bool ShowIcon { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        public override string ToString() {
            return $"{{{nameof(DateAdded)}={DateAdded}, {nameof(Guid)}={Guid}, {nameof(Id)}={Id}, {nameof(Name)}={Name}, {nameof(ShowIcon)}={ShowIcon.ToString()}, {nameof(Source)}={Source}, {nameof(Type)}={Type}, {nameof(Url)}={Url}}}";
        }
    }

    /// <summary>
    /// 从 json list 解析 Bookmark
    /// </summary>
    /// <param name="bookmarkJsonList"></param>
    /// <returns></returns>
    private List<Bookmark> getBookmarksFromJsonList(string bookmarkJsonList) {
        return JsonConvert.DeserializeObject<List<Bookmark>>(bookmarkJsonList) ?? new();
    }

    /// <summary>
    /// 解析 Bookmarks
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private List<KeyValuePair<string, List<Bookmark>>> parseBookmark(string path) {
        var rootToken = JObject.Parse(File.ReadAllText(path))["roots"];
        if (rootToken is null) {
            return new();
        }
        // bookmark_bar
        var FavoritesBarToken = rootToken["bookmark_bar"];
        // other
        var otherToken = rootToken["other"];
        if (FavoritesBarToken is null) {
            return new();
        }
        if (otherToken is null) {
            return new();
        }
        var bookmarks = new List<KeyValuePair<string, List<Bookmark>>>();
        // 未加入目录的链接
        var untitledFolder = new List<Bookmark>();
        bookmarks.Add(new("Favorites Bar", getBookmarksFromJsonList(FavoritesBarToken["children"]?.ToString() ?? "")));
        bookmarks.Add(new(nameof(untitledFolder), untitledFolder));
        // 解析 otherToken
        foreach (var foler in otherToken["children"]?.AsEnumerable() ?? Array.Empty<JToken>()) {
            // 目录
            if (foler["type"]?.ToString() == "folder") {
                bookmarks.Add(new(foler["name"]?.ToString() ?? "", getBookmarksFromJsonList(foler["children"]?.ToString() ?? "")));
            } else {
                if (getBookmarksFromJsonList($"[{foler.ToString()}]").FirstOrDefault() is Bookmark bookmark) {
                    untitledFolder.Add(bookmark);
                }
            }
        }
        return bookmarks;
    }

    /// <summary>
    /// 正确化 Bookmarks
    /// </summary>
    /// <param name="rawData"></param>
    /// <returns></returns>
    private List<KeyValuePair<string, List<Bookmark>>> correctBookmarks(List<KeyValuePair<string, List<Bookmark>>> rawData) {
        var bookmarks = new List<KeyValuePair<string, List<Bookmark>>>(rawData.Count);
        var folderNameDict = new Dictionary<string, List<Bookmark>>(rawData.Count);
        foreach (var folder in rawData) {
            var folderName = correctSheetName(folder.Key);
            if (!folderNameDict.ContainsKey(folderName)) {
                var list = new List<Bookmark>();
                folderNameDict.Add(folderName, list);
                bookmarks.Add(new(folderName, list));
            }
            folderNameDict[folderName].AddRange(mapper.Map<List<Bookmark>>(folder.Value));
        }
        return bookmarks;
    }

    private static readonly IList<char> InvalidSheetNameCharacters = new List<char>() { '\\', '/', '*', '?', ':', '[', ']', };
    private static readonly char InvalidSheetNameCharactersReplacement = ' ';

    /// <summary>
    /// 正确化 SheetName
    /// </summary>
    /// <param name="folderName"></param>
    /// <returns></returns>
    private string correctSheetName(string folderName) {
        for (int i = 0; i < InvalidSheetNameCharacters.Count; i++) {
            folderName = folderName.Replace(InvalidSheetNameCharacters[i], InvalidSheetNameCharactersReplacement);
        }
        // 不能超过 31 个字符
        return folderName[0..Math.Min(folderName.Length, 31)];
    }

    /// <summary>
    /// 导出 Bookmarks
    /// </summary>
    /// <param name="path"></param>
    /// <param name="outPath"></param>
    public void ExportBookmarks(string path, string outPath) {
        var fieldInfos = typeof(Bookmark).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var bookmarks = correctBookmarks(parseBookmark(path));
        #region write to excel
        var workbook = new XSSFWorkbook();
        foreach (var folder in bookmarks) {
            var sheet = workbook.CreateSheet($"{folder.Key}");
            #region create header
            var header = sheet.CreateRow(0);
            for (int i = 0; i < fieldInfos.Length; i++) {
                header.CreateCell(i).SetCellValue(fieldInfos[i].Name);
            }
            #endregion
            #region populate data
            for (int rowNum = 0; rowNum < folder.Value.Count; rowNum++) {
                var row = sheet.CreateRow(rowNum + 1);
                for (int i = 0; i < fieldInfos.Length; i++) {
                    row.CreateCell(i).SetCellValue(fieldInfos[i].GetValue(folder.Value[rowNum])?.ToString() ?? "");
                }
            }
            #endregion
        }
        workbook.Write(new FileStream(outPath, FileMode.Create));
        workbook.Close();
        #endregion
    }
}
