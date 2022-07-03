using CommonUITools.Utils;
using CommonUtil.Model;
using Downloader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace CommonUtil.Core;

public class Downloader {
    /// <summary>
    /// 下载配置
    /// </summary>
    private static readonly DownloadConfiguration DownloadConfiguration = new() { OnTheFlyDownload = false };
    /// <summary>
    /// 下载任务列表 dict，用于更新，与 DownloadTaskInfoList 同步更新
    /// </summary>
    public static readonly IDictionary<DownloadService, DownloadTask> DownloadTaskInfoDict = new Dictionary<DownloadService, DownloadTask>();
    /// <summary>
    /// 更新进度视图间隔时间
    /// </summary>
    public const short UpdateProcessInterval = 500;

    public static event EventHandler<DownloadTask>? DownloadCompleted;
    public static event EventHandler<DownloadTask>? DownloadFailed;

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="url"></param>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static DownloadTask Download(string url, DirectoryInfo directory) {
        var downloader = new DownloadService(DownloadConfiguration);
        downloader.DownloadStarted += DownloadStartedHandler;
        downloader.DownloadProgressChanged += DownloadProgressChangedHandler;
        downloader.DownloadFileCompleted += DownloadFileCompletedHandler;
        downloader.DownloadFileTaskAsync(url, directory);
        return new DownloadTask(url) {
            FileName = new Uri(url).Segments.LastOrDefault() ?? "未知文件名"
        };
    }

    /// <summary>
    /// 下载文件开始
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void DownloadStartedHandler(object? sender, DownloadStartedEventArgs e) {
        if (sender is DownloadService service) {
            var taskInfo = DownloadTaskInfoDict[service];
            UIUtils.RunOnUIThread(() => {
                taskInfo.TotalSize = e.TotalBytesToReceive;
                taskInfo.FileName = e.FileName;
            });
        }
    }

    /// <summary>
    /// 下载完成事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void DownloadFileCompletedHandler(object? sender, AsyncCompletedEventArgs e) {
        if (sender is DownloadService service) {
            var taskInfo = DownloadTaskInfoDict[service];
            // 更新视图
            UIUtils.RunOnUIThread(() => {
                taskInfo.LastUpdateTime = DateTime.Now;
                taskInfo.DownloadedSize = taskInfo.TotalSize;
                taskInfo.Process = 100;
                taskInfo.FinishTime = DateTime.Now;
                taskInfo.IsFinished = true;
                // 从下载列表中移除
                DownloadTaskInfoDict.Remove(service);
            });
            // 下载失败
            if (e.Error is null) {
                DownloadFailed?.Invoke(null, taskInfo);
            } else {
                DownloadCompleted?.Invoke(null, taskInfo);
            }
        }
    }

    /// <summary>
    /// 下载进度事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void DownloadProgressChangedHandler(object? sender, DownloadProgressChangedEventArgs e) {
        if (sender is DownloadService service) {
            var taskInfo = DownloadTaskInfoDict[service];
            // 未到更新时间
            if ((DateTime.Now - taskInfo.LastUpdateTime).TotalMilliseconds <= UpdateProcessInterval) {
                return;
            }
            // 更新视图
            UIUtils.RunOnUIThread(() => {
                taskInfo.LastUpdateTime = DateTime.Now;
                taskInfo.DownloadedSize = e.ReceivedBytesSize;
                taskInfo.DownloadSpeed = e.BytesPerSecondSpeed;
                taskInfo.Process = (byte)e.ProgressPercentage;
            });
        }
    }

}
