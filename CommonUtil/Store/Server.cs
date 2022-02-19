using CommonUtil.Utils;
using Flurl.Http;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CommonUtil.Store;

public class Server {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// nodejs 服务
    /// </summary>
    public static Process? NodejsProcess { get; private set; }
    /// <summary>
    /// nodejs 服务端口
    /// </summary>
    public static int? NodeJsServerPort { get; private set; }
    /// <summary>
    /// nodejs server 网址前缀，不包含 '/'
    /// </summary>
    public static string? NodeJsServerBaseUrl { get; private set; }
    private static readonly object NodeJsServerLock = new();
    /// <summary>
    /// nodejs 可执行文件路径
    /// </summary>
    private static readonly string NodeJsServerFilePath = Path.Combine(Global.ApplicationPath, "resource/lib/nodejs/server.exe");
    private static readonly short MinNodeJsServerPort = 3001; // 允许的最小端口号
    private static readonly short NodejsHeartbeatInterval = 1000; // 每隔此时间发送一次连接
    private static Timer? NodejsHeartbeatTimer;
    /// <summary>
    /// 存储临时 NodeJsServerPort 路径
    /// </summary>
    private static readonly string NodeJsServerPortCacheFile = Path.Combine(Global.CacheDirectory, "nodejsport.cache");
    /// <summary>
    /// 检查 Nodejs 服务器是否启动，未启动则开启服务
    /// </summary>
    public static void CheckNodeJsServer() {
        if (NodeJsServerPort != null) {
            return;
        }
        lock (NodeJsServerLock) {
            if (NodeJsServerPort != null) {
                return;
            }
            // 先删除 cache 文件
            CommonUtils.Try(() => File.Delete(NodeJsServerPortCacheFile));
            // 手动启动
            if (Config.Environment == Model.Environment.Development) {
                string cmd = $"./node_modules/.bin/ts-node ./main/index.ts path=\"{NodeJsServerPortCacheFile}\"";
                Console.WriteLine($"Please go to NodejsService root folder, open the terminal, and type {cmd}");
            } else {
                // 启动 nodejsserver
                var process = new Process();
                process.StartInfo.FileName = NodeJsServerFilePath;
                process.StartInfo.Arguments = $"path=\"{NodeJsServerPortCacheFile}\"";
                process.StartInfo.CreateNoWindow = true;
                process.Start();
            }
            // 检查服务是否启动
            // 不断检查服务是否写入 port
            int port = 0;
            while (true) {
                if (File.Exists(NodeJsServerPortCacheFile)) {
                    if (int.TryParse(File.ReadAllText(NodeJsServerPortCacheFile), out port)) {
                        // 成功启动
                        if (port >= MinNodeJsServerPort) {
                            NodeJsServerPort = port;
                            NodeJsServerBaseUrl = $"http://localhost:{NodeJsServerPort}";
                            break;
                        }
                    }
                }
                Thread.Sleep(50);
            }
            // 服务已启动
            StartNodejsHeartbeat();
            // 退出 hook
            App.Current.Exit += (o, e) => NodejsProcess?.Close();
        }
    }

    /// <summary>
    /// nodejs 心跳机制
    /// </summary>
    /// <see cref="NodejsHeartbeatInterval"/>
    private static void StartNodejsHeartbeat() {
        NodejsHeartbeatTimer = new Timer(async o => {
            try {
                var resp = await $"http://localhost:{NodeJsServerPort}/heartbeat".GetAsync();
                if (Config.Environment == Model.Environment.Development) {
                    Logger.Debug($"Nodejs heartbeat, result code: {resp.StatusCode}");
                }
            } catch (Exception error) {
                Logger.Info(error);
            }
        }, null, 0, NodejsHeartbeatInterval);
    }
}
