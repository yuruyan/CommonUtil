using CommonUITools.Utils;
using NLog;
using NPOI.HPSF;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CommonUtil.Core;

public class KeywordResult {
    /// <summary>
    /// 文件全路径
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    public KeywordResult(string filename) {
        Filename = filename;
    }
}

public class KeywordFinder {
    private const int ThreadCount = 8;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public string SearchDirectory { get; private set; }
    public IEnumerable<string> ExcludeDirectoryRegexes { get; private set; }
    public IEnumerable<string> ExcludeFileRegexes { get; private set; }
    /// <summary>
    /// 文件内容字典
    /// </summary>
    private readonly IDictionary<string, string> FileDataDict = new Dictionary<string, string>();
    private readonly ConcurrentQueue<KeywordResult> FindResultQueue = new();
    /// <summary>
    /// 更新结果定时器
    /// </summary>
    private readonly System.Timers.Timer UpdateResultTimer = new(250);
    /// <summary>
    /// 结果集合
    /// </summary>
    private ICollection<KeywordResult> ResultCollection = new List<KeywordResult>();
    /// <summary>
    /// 查找任务线程集合
    /// </summary>
    private readonly List<KeyValuePair<Task, CancellationTokenSource>> KeywordFindingTaskList = new();
    /// <summary>
    /// 文件加载任务线程集合
    /// </summary>
    private readonly List<KeyValuePair<Task, CancellationTokenSource>> FileDataLoaderTaskList = new();
    private CancellationTokenSource KeywordFindingTaskWaiterCancellationTokenSource = new();
    private CancellationTokenSource FileDataLoaderTaskWaiterCancellationTokenSource = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="searchDirectory">搜索目录</param>
    /// <param name="excludeDirectoryRegexes">排除目录正则</param>
    /// <param name="excludeFileRegexes">排除文件正则</param>
    public KeywordFinder(string searchDirectory, IEnumerable<string>? excludeDirectoryRegexes = null, IEnumerable<string>? excludeFileRegexes = null) {
        Init();
        SearchDirectory = searchDirectory;
        ExcludeDirectoryRegexes = excludeDirectoryRegexes ?? new List<string>();
        ExcludeFileRegexes = excludeFileRegexes ?? new List<string>();
    }

    private void Init() {
        UpdateResultTimer.Elapsed += (o, e) => {
            if (FindResultQueue.IsEmpty) {
                return;
            }
            UIUtils.RunOnUIThread(() => {
                // 全部出队
                while (FindResultQueue.TryDequeue(out var result)) {
                    ResultCollection.Add(result);
                }
            });
        };
        UpdateResultTimer.Start();
    }

    /// <summary>
    /// 获取指定文件列表所有内容，文件内容会自动缓存
    /// </summary>
    /// <param name="filenames">文件路径</param>
    /// <returns>[filename,content]</returns>
    private Dictionary<string, string> GetFileData(IEnumerable<string> filenames) {
        var fileDataDict = new Dictionary<string, string>();
        int perThreadFilesCount = (int)Math.Ceiling(filenames.Count() / (double)ThreadCount);
        var taskFilenameList = filenames.Split(ThreadCount);
        // 加载文件
        foreach (var filenameList in taskFilenameList) {
            var cancellationTokenSource = new CancellationTokenSource();
            var task = Task.Run(() => {
                var tempDict = new Dictionary<string, string>();
                foreach (var filename in filenameList) {
                    if (cancellationTokenSource.IsCancellationRequested) {
                        break;
                    }
                    // 已经加载过
                    if (FileDataDict.ContainsKey(filename)) {
                        tempDict[filename] = FileDataDict[filename];
                        continue;
                    }
                    // 加载文件数据
                    try {
                        tempDict[filename] = File.ReadAllText(filename);
                    } catch {
                        Logger.Info($"读取文件 {filename} 失败");
                    }
                }
                // 一并填充数据
                lock (this) {
                    foreach (var item in tempDict) {
                        var name = item.Key;
                        fileDataDict[name] = item.Value;
                        // 缓存
                        if (!FileDataDict.ContainsKey(name)) {
                            FileDataDict[name] = tempDict[name];
                        }
                    }
                }
            }, cancellationTokenSource.Token);
            FileDataLoaderTaskList.Add(new(task, cancellationTokenSource));
        }
        FileDataLoaderTaskWaiterCancellationTokenSource = new();
        // 等待加载完毕
        Task.WaitAll(
           FileDataLoaderTaskList.Select(t => t.Key).ToArray(),
           FileDataLoaderTaskWaiterCancellationTokenSource.Token
       );
        return fileDataDict;
    }

    /// <summary>
    /// 终止当前查询
    /// </summary>
    public void CancelFinding() {
        UpdateResultTimer.Stop();
        KeywordFindingTaskWaiterCancellationTokenSource.Cancel();
        FileDataLoaderTaskWaiterCancellationTokenSource.Cancel();
        // 终止查询线程
        foreach (var item in KeywordFindingTaskList) {
            item.Value.Cancel();
        }
        // 终止文件加载线程
        foreach (var item in FileDataLoaderTaskList) {
            item.Value.Cancel();
        }
        KeywordFindingTaskList.Clear();
        FileDataLoaderTaskList.Clear();
        FindResultQueue.Clear();
    }

    /// <summary>
    /// 查找关键字
    /// </summary>
    /// <param name="keywordRegex">查找的正则</param>
    /// <param name="excludeDirRegexes"></param>
    /// <param name="excludeFileRegexes"></param>
    /// <param name="results"></param>
    /// <returns></returns>
    public void FindKeyword(
        string keywordRegex,
        IEnumerable<string> excludeDirRegexes,
        IEnumerable<string> excludeFileRegexes,
        ICollection<KeywordResult> results
    ) {
        CancelFinding();
        UpdateResultTimer.Start();
        ResultCollection = results;
        // 清空
        UIUtils.RunOnUIThread(() => ResultCollection.Clear());
        // 加载文件内容
        var fileDataDict = GetFileData(FilterFiles(SearchDirectory, excludeDirRegexes, excludeFileRegexes));
        var keywordCompiledRegex = CompileRegex(new string[] { keywordRegex }).First();
        // 文件名列表
        var filenameList = fileDataDict.Keys.Split(ThreadCount);
        // 查找
        foreach (var list in filenameList) {
            var cancellationTokenSource = new CancellationTokenSource();
            // 启动查找任务线程
            var task = Task.Run(() => {
                foreach (var filename in list) {
                    if (cancellationTokenSource.IsCancellationRequested) {
                        break;
                    }
                    if (!keywordCompiledRegex.IsMatch(FileDataDict[filename])) {
                        continue;
                    }
                    FindResultQueue.Enqueue(new(filename));
                }
            }, cancellationTokenSource.Token);
            KeywordFindingTaskList.Add(new(task, cancellationTokenSource));
        }
        KeywordFindingTaskWaiterCancellationTokenSource = new();
        Task.WaitAll(
            KeywordFindingTaskList.Select(t => t.Key).ToArray(),
            KeywordFindingTaskWaiterCancellationTokenSource.Token
        );
    }

    /// <summary>
    /// 筛选目录和文件
    /// </summary>
    /// <param name="excludeDirs"></param>
    /// <param name="excludeFiles"></param>
    /// <returns></returns>
    private IEnumerable<string> FilterDirectoryFiles(IEnumerable<string> excludeDirs, IEnumerable<string> excludeFiles) {
        var excludeFileRegexes = CompileRegex(excludeFiles);
        var excludeDirRegexes = CompileRegex(excludeDirs);
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
    /// <param name="excludeDirRegexes"></param>
    /// <param name="excludeFileRegexes"></param>
    /// <returns>筛选后的文件路径</returns>
    private IEnumerable<string> FilterFiles(string directory, IEnumerable<string> excludeDirRegexes, IEnumerable<string> excludeFileRegexes) {
        var filenames = new List<string>();
        var results = new List<string>();
        // 编译正则
        var excludeCompiledDirs = CompileRegex(excludeDirRegexes);
        var excludeCompiledFiles = CompileRegex(excludeFileRegexes);
        // 获取筛选目录后的文件
        GetAllFilenames(directory, filenames, excludeCompiledDirs);
        // 获取筛选后的文件
        return filenames.Where(
            f => !excludeCompiledFiles.Any(re => re.IsMatch(f))
        );
    }

    /// <summary>
    /// 编译正则
    /// </summary>
    /// <param name="regexes"></param>
    /// <returns></returns>
    private IEnumerable<Regex> CompileRegex(IEnumerable<string> regexes) {
        return regexes.Select(
            r => new Regex(r, RegexOptions.IgnoreCase | RegexOptions.ECMAScript)
        );
    }

    /// <summary>
    /// 获取文件夹下所有的文件路径
    /// </summary>
    /// <param name="directory">要搜索的文件夹</param>
    /// <param name="filenames">获取的文件路径添加到此</param>
    /// <param name="excludeDirRegex">排除的文件夹正则</param>
    private void GetAllFilenames(string directory, IList<string> filenames, IEnumerable<Regex> excludeDirRegex) {
        foreach (var item in Directory.GetFiles(directory)) {
            filenames.Add(item);
        }
        foreach (var dir in Directory.GetDirectories(directory)) {
            // 排除
            if (excludeDirRegex.Any(re => re.IsMatch(dir))) {
                continue;
            }
            // 递归遍历文件
            GetAllFilenames(dir, filenames, excludeDirRegex);
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
