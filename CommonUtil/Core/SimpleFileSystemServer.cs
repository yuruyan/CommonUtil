using System;
using System.Threading.Tasks;
using CommonUITools.Model;
using CommonUITools.Utils;
using Flurl.Http;
using System.Diagnostics;
using System.Windows;
using System.Threading;

namespace CommonUtil.Core;

public class SimpleFileSystemServer {
    /// <summary>
    /// Server App 路径
    /// </summary>
    public static readonly string AppPath = "";
    public int Port { get; }
    public string WordkingDirectory { get; }

    private Process? ServerProcess;
    private System.Timers.Timer HeartBeatTimer;
    /// <summary>
    /// 心跳间隔时间
    /// </summary>
    private static readonly int HeartBeatInterval = 250;
    /// <summary>
    /// 进程是否启动
    /// </summary>
    private bool IsStarted;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="port">端口号</param>
    /// <param name="dir">工作目录</param>
    public SimpleFileSystemServer(int port, string dir) {
        Port = port;
        WordkingDirectory = dir;
        #region 初始化 HeartBeatTimer
        HeartBeatTimer = new(HeartBeatInterval);
        HeartBeatTimer.Elapsed += async (s, e) => {
            GeneralResponse? resp = null;
            resp = await CommonUtils.TryAsync(() => $"http://localhost:{Port}".GetJsonAsync<GeneralResponse>());
            // 启动成功
            if (resp != null && resp.code == 200) {
                IsStarted = true;
                HeartBeatTimer.Stop();
            }
        };
        #endregion
        // 主进程结束，自动结束子进程
        Application.Current.Exit += (s, e) => Stop();
    }

    /// <summary>
    /// 异步启动文件服务器
    /// </summary>
    /// <param name="waitTime">ms, 等待多长时间未启动则取消</param>
    public Task<bool> StartAsync(int waitTime = 5000) {
        // 已经有服务器在运行
        if (ServerProcess is not null) {
            return Task.Run(() => true);
        }
        ServerProcess = Process.Start(AppPath, $" --urls=http://*:{Port} --dir=\"{WordkingDirectory}\"");
        HeartBeatTimer.Start();
        return Task.Run(() => {
            var beginTime = DateTime.Now;
            // 判断是否启动成功
            while (true) {
                // 超时结束
                if ((DateTime.Now - beginTime).TotalMilliseconds > waitTime) {
                    Stop();
                    return false;
                }
                // 进程正常启动
                if (IsStarted) {
                    return true;
                }
                // 进程终止
                if (ServerProcess.HasExited) {
                    return false;
                }
                Thread.Sleep(HeartBeatInterval);
            }
        });
    }

    /// <summary>
    /// 停止服务器
    /// </summary>
    public void Stop() {
        ServerProcess?.Kill();
        ServerProcess = null;
        IsStarted = false;
    }
}
