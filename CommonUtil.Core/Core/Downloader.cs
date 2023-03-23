using Downloader;
using System.Net;
using DownloadProgressChangedEventArgs = Downloader.DownloadProgressChangedEventArgs;

namespace CommonUtil.Core;

public class Downloader {
    /// <summary>
    /// 下载配置
    /// </summary>
    private static DownloadConfiguration DefaultDownloadConfiguration => new();
    /// <summary>
    /// 下载任务列表 dict，用于更新，与 DownloadTaskInfoList 同步更新
    /// </summary>
    public readonly IDictionary<DownloadService, DownloadTask> DownloadTaskInfoDict = new Dictionary<DownloadService, DownloadTask>();
    /// <summary>
    /// 更新进度视图间隔时间
    /// </summary>
    public const short UpdateProcessInterval = 500;
    private readonly Debounce DownloadProgressDebounce = new(callRegular: true);
    public event EventHandler<DownloadTask>? DownloadCompleted;
    public event EventHandler<DownloadTask>? DownloadFailed;

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="url"></param>
    /// <param name="directory"></param>
    /// <param name="proxy">代理</param>
    /// <returns>url 无效返回 null</returns>
    public DownloadTask? Download(string url, DirectoryInfo directory, WebProxy? proxy = null) {
        var options = DefaultDownloadConfiguration;
        options.RequestConfiguration = new() {
            Proxy = proxy,
        };
        var service = new DownloadService(options);
        service.DownloadStarted += DownloadStartedHandler;
        service.DownloadProgressChanged += DownloadProgressChangedHandler;
        service.DownloadFileCompleted += DownloadFileCompletedHandler;
        // 无效 url
        if (TaskUtils.Try(() => new Uri(url)) is not Uri uri) {
            return null;
        }
        var downloadTask = new DownloadTask(url, directory, uri.Segments.LastOrDefault() ?? "未知文件名") {
            Proxy = proxy,
            Status = ProcessResult.Processing,
        };
        DownloadTaskInfoDict[service] = downloadTask;
        service.DownloadFileTaskAsync(url, directory);
        return downloadTask;
    }

    /// <summary>
    /// 下载文件开始
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DownloadStartedHandler(object? sender, DownloadStartedEventArgs e) {
        if (sender is DownloadService service) {
            var taskInfo = DownloadTaskInfoDict[service];
            UIUtils.RunOnUIThread(() => {
                taskInfo.FileSize = e.TotalBytesToReceive;
                taskInfo.FileName = Path.GetFileName(e.FileName);
            });
        }
    }

    /// <summary>
    /// 下载完成事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DownloadFileCompletedHandler(object? sender, AsyncCompletedEventArgs e) {
        if (sender is not DownloadService service) {
            return;
        }
        var taskInfo = DownloadTaskInfoDict[service];
        // 更新视图 
        UIUtils.RunOnUIThread(() => {
            taskInfo.FileSize = service.Package.ReceivedBytesSize;
            taskInfo.LastUpdateTime = DateTime.Now;
            taskInfo.DownloadedSize = taskInfo.FileSize;
            taskInfo.Process = 100;
            taskInfo.FinishTime = DateTime.Now;
            taskInfo.Status = e.Error is null ? ProcessResult.Successful : ProcessResult.Failed;
            // 从下载列表中移除
            DownloadTaskInfoDict.Remove(service);
        });
        // 下载成功
        if (e.Error is null) {
            DownloadCompleted?.Invoke(null, taskInfo);
        } else {
            DownloadFailed?.Invoke(null, taskInfo);
        }
    }

    /// <summary>
    /// 下载进度事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DownloadProgressChangedHandler(object? sender, DownloadProgressChangedEventArgs e) {
        if (sender is not DownloadService service) {
            return;
        }
        DownloadProgressDebounce.Run(() => {
            if (!DownloadTaskInfoDict.TryGetValue(service, out var taskInfo)) {
                return;
            }
            // 更新视图
            UIUtils.RunOnUIThread(() => {
                taskInfo.LastUpdateTime = DateTime.Now;
                taskInfo.DownloadedSize = e.ReceivedBytesSize;
                taskInfo.DownloadSpeed = e.BytesPerSecondSpeed;
                taskInfo.Process = (byte)e.ProgressPercentage;
            });
        });
    }
}
