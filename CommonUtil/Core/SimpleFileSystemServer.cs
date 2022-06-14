using System;
using System.Threading.Tasks;
using CommonUITools.Model;
using CommonUITools.Utils;
using Flurl.Http;
using System.Diagnostics;
using System.Windows;
using System.Threading;
using NLog;

namespace CommonUtil.Core;

public class SimpleFileSystemServer {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// Server App 路径
    /// </summary>
    public static readonly string AppPath = "SimpleFileSystemServer.exe";
    public int Port { get; }
    public string WordkingDirectory { get; }
    /// <summary>
    /// 服务器停止事件
    /// </summary>
    public event EventHandler Stopped;
    /// <summary>
    /// 服务器启动事件
    /// </summary>
    public event EventHandler Started;
    /// <summary>
    /// 服务器进程
    /// </summary>
    private Process? ServerProcess;
    /// <summary>
    /// 检测服务器是否启动定时器
    /// </summary>
    private readonly System.Timers.Timer CheckStartedTimer;
    /// <summary>
    /// 心跳机制定时器，检测服务器是否宕机
    /// </summary>
    private readonly System.Timers.Timer HeartBeatTimer;
    /// <summary>
    /// 最长未响应次数，超过此值则认为服务器已经关闭
    /// </summary>
    private static readonly byte MaxNoResponseTimes = 16;
    /// <summary>
    /// 当前未响应次数
    /// </summary>
    private static byte NoResponseTimes;
    /// <summary>
    /// 心跳间隔时间
    /// </summary>
    private static readonly int HeartBeatInterval = 250;
    /// <summary>
    /// 进程是否启动
    /// </summary>
    public bool IsStarted { get; private set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="port">端口号</param>
    /// <param name="dir">分享目录</param>
    public SimpleFileSystemServer(int port, string dir) {
        Port = port;
        WordkingDirectory = dir;
        #region 初始化 CheckStartedTimer、HeartBeatTimer
        CheckStartedTimer = new(HeartBeatInterval);
        CheckStartedTimer.Elapsed += async (s, e) => {
            GeneralResponse? resp = null;
            resp = await CommonUtils.TryAsync(() => $"http://localhost:{Port}/heartbeat".GetJsonAsync<GeneralResponse>());
            // 启动成功
            if (resp != null && resp.code == 200) {
                IsStarted = true;
                CheckStartedTimer.Stop();
            }
        };
        HeartBeatTimer = new(HeartBeatInterval);
        HeartBeatTimer.Elapsed += async (s, e) => {
            GeneralResponse? resp = null;
            resp = await CommonUtils.TryAsync(() => $"http://localhost:{Port}/heartbeat".GetJsonAsync<GeneralResponse>());
            // 成功
            if (resp != null && resp.code == 200) {
                // 清零
                NoResponseTimes = 0;
            } else {
                NoResponseTimes++;
                Logger.Debug($"HeartBeat {nameof(SimpleFileSystemServer)} retries {NoResponseTimes} times");
            }
            // 超过最长未响应时长
            if (NoResponseTimes >= MaxNoResponseTimes) {
                Logger.Info($"HeartBeat {nameof(SimpleFileSystemServer)} retries timeout");
                Stop();
            }
        };
        #endregion
        // 主进程结束，自动结束子进程
        Application.Current.Exit += (s, e) => Stop();
        // 设置启动、退出事件监听程序
        Started += (s, e) => {
            HeartBeatTimer.Start();
            Logger.Debug($"{nameof(SimpleFileSystemServer)} has started");
        };
        Stopped += (s, e) => {
            HeartBeatTimer.Stop();
            Logger.Debug($"{nameof(SimpleFileSystemServer)} has stopped");
        };
    }

    /// <summary>
    /// 异步启动文件服务器
    /// </summary>
    /// <param name="waitTime">ms, 等待多长时间未启动则取消</param>
    public async Task<bool> StartAsync(int waitTime = 5000) {
        // 已经有服务器在运行
        if (ServerProcess is not null) {
            return true;
        }
        ServerProcess = Process.Start(new ProcessStartInfo(AppPath, $" --urls=http://*:{Port} --dir=\"{WordkingDirectory}\"") {
            CreateNoWindow = true
        });
        if (ServerProcess is null) {
            return false;
        }
        CheckStartedTimer.Start();
        return await Task.Run(() => {
            var beginTime = DateTime.Now;
            // 判断是否启动成功
            while (true) {
                // 超时结束
                if ((DateTime.Now - beginTime).TotalMilliseconds > waitTime) {
                    Logger.Info($"Start {nameof(SimpleFileSystemServer)} timeout");
                    Stop();
                    return false;
                }
                // 进程正常启动
                if (IsStarted) {
                    Started?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                // 进程终止
                if (ServerProcess.HasExited) {
                    Logger.Info($"{nameof(SimpleFileSystemServer)} exit with code {ServerProcess.ExitCode}");
                    Stop();
                    return false;
                }
                Thread.Sleep(HeartBeatInterval);
            }
        });
    }

    /// <summary>
    /// 停止服务器，ServerProcess 设置为 null
    /// </summary>
    public void Stop() {
        if (ServerProcess is null) {
            return;
        }
        CommonUtils.Try(() => ServerProcess?.Kill());
        ServerProcess = null;
        CheckStartedTimer.Stop();
        IsStarted = false;
        Stopped?.Invoke(this, EventArgs.Empty);
    }
}
