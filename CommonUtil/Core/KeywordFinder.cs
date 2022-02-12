using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CommonUtil.Core;

public class KeywordResult {
    /// <summary>
    /// 文件路径
    /// </summary>
    public string File { get; set; } = string.Empty;
    /// <summary>
    /// 结果(line,column)
    /// </summary>
    public List<KeyValuePair<int, int>> MatchList { get; set; } = new();
}

public class KeywordFinder {
    private static readonly int ThreadCount = 16;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public string SearchDirectory { get; private set; }
    public List<string> ExcludeDirectoryRegexes { get; private set; }
    public List<string> ExcludeFileRegexes { get; private set; }
    private readonly IDictionary<string, IList<string>> FileDataDict = new Dictionary<string, IList<string>>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="searchDirectory">搜索目录</param>
    /// <param name="excludeDirectoryRegexes">排除目录正则</param>
    /// <param name="excludeFileRegexes">排除文件正则</param>
    public KeywordFinder(string searchDirectory, List<string>? excludeDirectoryRegexes = null, List<string>? excludeFileRegexes = null) {
        SearchDirectory = searchDirectory;
        ExcludeDirectoryRegexes = excludeDirectoryRegexes ?? new List<string>();
        ExcludeFileRegexes = excludeFileRegexes ?? new List<string>();
        FileDataDict = GetFileData(FilterFiles(SearchDirectory, ExcludeDirectoryRegexes, ExcludeFileRegexes));
    }

    /// <summary>
    /// 获取文件所有内容
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    private Dictionary<string, IList<string>> GetFileData(List<string> files) {
        var dataList = new Dictionary<string, IList<string>>();
        int perThreadCount = (int)Math.Ceiling(files.Count / (double)ThreadCount);
        var tasks = new List<Task>(ThreadCount);
        // 文件总数小于等于线程数
        if (files.Count <= ThreadCount) {
            foreach (var file in files) {
                tasks.Add(Task.Factory.StartNew(() => {
                    try {
                        lock (this) {
                            dataList[file] = File.ReadAllLines(file);
                        }
                    } catch (Exception e) {
                        Logger.Error(e);
                    }
                }));
            }
        } else {
            for (int i = 0; i < ThreadCount; i++) {
                int tempI = i;
                tasks.Add(Task.Factory.StartNew(() => {
                    for (int j = tempI * perThreadCount; j < Math.Min(files.Count, (tempI + 1) * perThreadCount); j++) {
                        try {
                            lock (this) {
                                dataList[files[j]] = File.ReadAllLines(files[j]);
                            }
                        } catch (Exception e) {
                            Logger.Error(e);
                        }
                    }
                }));
            }
        }
        Task.WaitAll(tasks.ToArray());
        return dataList;
    }

    /// <summary>
    /// 查找关键字
    /// </summary>
    /// <param name="keywordRegex">查找的正则</param>
    /// <param name="results"></param>
    /// <param name="dispatcher"></param>
    /// <returns></returns>
    public void FindKeyword(string keywordRegex, List<string> excludeDirs, List<string> excludeFiles, ObservableCollection<KeywordResult> results, Dispatcher? dispatcher = null) {
        // 加载必要文件
        var dict = GetFileData(FilterFiles(SearchDirectory, excludeDirs, excludeFiles));
        foreach (var item in dict) {
            FileDataDict[item.Key] = item.Value;
        }
        var re = new Regex(keywordRegex, RegexOptions.IgnoreCase);
        var tasks = new List<Task>(ThreadCount); // 线程集合
        var keyList = new List<List<string>>(ThreadCount); // 线程进行处理的数据
        var filterKeys = FilterDirectoryFiles(excludeDirs, excludeFiles).ToArray();
        int perThreadCount = (int)Math.Ceiling(filterKeys.Length / (double)ThreadCount); // 每个线程进行处理的数据数目
        if (dispatcher != null) {
            dispatcher.Invoke(() => results.Clear());
        } else {
            results.Clear();
        }
        // 初始化集合
        for (int i = 0; i < ThreadCount; i++) {
            keyList.Add(new());
        }
        // 拆分集合
        if (filterKeys.Length <= ThreadCount) {
            for (int i = 0; i < filterKeys.Length; i++) {
                keyList[i].Add(filterKeys[i]);
            }
        } else {
            for (int i = 0; i < ThreadCount; i++) {
                keyList[i] = filterKeys[Math.Min(i * perThreadCount, filterKeys.Length)..Math.Min((i + 1) * perThreadCount, filterKeys.Length)].ToList();
            }
        }
        // 查找
        foreach (var list in keyList) {
            Task task = Task.Factory.StartNew(() => {
                foreach (var file in list) {
                    var matchList = new List<KeyValuePair<int, int>>();
                    bool found = false;
                    // 每一行进行匹配
                    for (int line = 0; line < FileDataDict[file].Count(); line++) {
                        if (re.IsMatch(FileDataDict[file][line])) {
                            found = true;
                            foreach (Match match in re.Matches(FileDataDict[file][line])) {
                                matchList.Add(new(line, match.Index));
                            }
                            // 只查找第一次匹配成功的行
                            break;
                        }
                    }
                    if (found) {
                        lock (this) {
                            if (dispatcher != null) {
                                dispatcher.Invoke(() => results.Add(new() { File = file, MatchList = matchList }));
                            } else {
                                results.Add(new() { File = file, MatchList = matchList });
                            }
                        }
                    }
                }
            });
            tasks.Add(task);
        }
        Task.WaitAll(tasks.ToArray());
    }

    /// <summary>
    /// 筛选目录和文件
    /// </summary>
    /// <param name="excludeDirs"></param>
    /// <param name="excludeFiles"></param>
    /// <returns></returns>
    private List<string> FilterDirectoryFiles(List<string> excludeDirs, List<string> excludeFiles) {
        List<Regex> excludeFileRegexes = CompileRegex(excludeFiles);
        List<Regex> excludeDirRegexes = CompileRegex(excludeDirs);
        var tempFiles = new List<string>();
        var tempFiles2 = new List<string>();
        // 筛选目录
        foreach (var file in FileDataDict.Keys) {
            bool found = false;
            foreach (var regex in excludeDirRegexes) {
                if (regex.IsMatch(GetRelativeDirectory(SearchDirectory, file))) {
                    found = true;
                    break;
                }
            }
            if (!found) {
                tempFiles.Add(file);
            }
        }
        // 筛选查找文件
        foreach (var file in tempFiles) {
            bool found = false;
            foreach (var regex in excludeFileRegexes) {
                if (regex.IsMatch(file)) {
                    found = true;
                    break;
                }
            }
            if (!found) {
                tempFiles2.Add(file);
            }
        }
        return tempFiles2;
    }

    /// <summary>
    /// 筛选文件
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="excludeDirectoryRegex"></param>
    /// <param name="excludeFileRegex"></param>
    /// <returns></returns>
    private List<string> FilterFiles(string directory, List<string> excludeDirectoryRegex, List<string> excludeFileRegex) {
        var files = new List<string>();
        var results = new List<string>();
        // 编译正则
        var excludeDirs = CompileRegex(excludeDirectoryRegex);
        var excludeFiles = CompileRegex(excludeFileRegex);
        GetAllFiles(directory, files, excludeDirs); // 获取所有文件
        // 排除文件
        foreach (var f in files) {
            bool found = false;
            foreach (var regex in excludeFiles) {
                if (regex.IsMatch(f)) {
                    found = true;
                    break;
                }
            }
            // 不匹配并且没有加载过
            if (!found && !FileDataDict.ContainsKey(f)) {
                results.Add(f);
            }
        }
        return results;
    }

    /// <summary>
    /// 编译正则
    /// </summary>
    /// <param name="regexes"></param>
    /// <returns></returns>
    private List<Regex> CompileRegex(List<string> regexes) {
        var results = new List<Regex>();
        foreach (var item in regexes) {
            results.Add(new Regex(item, RegexOptions.IgnoreCase));
        }
        return results;
    }

    /// <summary>
    /// 获取文件夹下所有的文件
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="files"></param>
    private void GetAllFiles(string directory, List<string> files, List<Regex>? excludeDir = null) {
        files.AddRange(Directory.GetFiles(directory));
        excludeDir ??= new List<Regex>();
        foreach (var dir in Directory.GetDirectories(directory)) {
            bool found = false;
            foreach (var regex in excludeDir) {
                if (regex.IsMatch(dir)) {
                    Console.WriteLine($"excluded: {dir}");
                    found = true;
                    break;
                }
            }
            if (!found) {
                GetAllFiles(dir, files, excludeDir);
            }
        }
    }

    /// <summary>
    /// 获取相对路径文件夹
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    private string GetRelativeDirectory(string directory, string file) {
        directory = directory.Replace("\\", "/").TrimEnd('/');
        file = file.Replace("\\", "/");
        return file[directory.Length..file.LastIndexOf('/')];
    }
}
