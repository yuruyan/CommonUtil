using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleFileSystemServer.Model;
using SimpleFileSystemServer.Service;

namespace SimpleFileSystemServer.Controllers;

[Route("")]
public class HomeController : Controller {
    private readonly FileService FileService = new();

    /// <summary>
    /// 列出文件列表
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    [HttpGet("/path")]
    [HttpGet("")]
    public ActionResult Index([FromQuery()] string path = "/") {
        path = "/" + (path ?? "").Trim('\\', '/');
        // 非法访问
        if (!FileService.CheckPathRange(path)) {
            throw new FileNotFoundException(path);
        }
        ViewData["data"] = JsonConvert.SerializeObject(new {
            currentPath = path,
            files = FileService.ListFiles(path)
        });
        return View("/views/index.cshtml");
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="path">文件相对路径</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    [HttpGet("/download")]
    public FileResult Donwload([FromQuery()] string path) {
        path = (path ?? "").Trim('\\', '/');
        // 非法访问
        if (!FileService.CheckPathRange(path)) {
            throw new FileNotFoundException(path);
        }
        var filePath = Path.Combine(Global.WorkingDirectory, path);
        return File(new FileStream(filePath, FileMode.Open, FileAccess.Read), "application/octet-stream", Path.GetFileName(filePath));
    }

    /// <summary>
    /// 心跳测试
    /// </summary>
    /// <returns></returns>
    [HttpGet("/heartbeat")]
    public GeneralResponse HeartBeat() {
        return new GeneralResponse() { code = 200, message = "success" };
    }
}
