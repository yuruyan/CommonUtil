using CommonUITools.Model;
using CommonUtil.Model;
using CommonUtil.Store;
using Flurl.Http;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace CommonUtil.Core;

public class FtpServer {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 异步启动 FtpServer
    /// </summary>
    /// <param name="config"></param>
    /// <returns>启动成功返回 true，失败返回 false</returns>
    public static async Task<bool> StartFtpServerAsync(FtpServerConfig config) {
        try {
            Server.CheckNodeJsServer();
            string respData = await (Server.NodeJsServerBaseUrl + "/startftpserver").PostJsonAsync(config).Result.GetStringAsync();
            JsonResponse? resp = JsonConvert.DeserializeObject<JsonResponse>(respData);
            if (resp == null) {
                throw new Exception("deserialize object failed");
            }
            return resp.code == 200;
        } catch (Exception e) {
            Logger.Info(e);
            return false;
        }
    }

    /// <summary>
    /// 异步关闭 FTP Server
    /// </summary>
    /// <returns></returns>
    public static async Task<bool> StopFtpServerAsync() {
        try {
            Server.CheckNodeJsServer();
            string respData = await (Server.NodeJsServerBaseUrl + "/stopftpserver").GetStringAsync();
            JsonResponse? resp = JsonConvert.DeserializeObject<JsonResponse>(respData);
            if (resp == null) {
                throw new Exception("deserialize object failed");
            }
            return resp.code == 200;
        } catch (Exception e) {
            Logger.Info(e);
            return false;
        }
    }
}
