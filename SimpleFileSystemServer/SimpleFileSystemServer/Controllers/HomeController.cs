using Microsoft.AspNetCore.Mvc;
using SimpleFileSystemServer.Service;

namespace SimpleFileSystemServer.Controllers;

[Route("")]
public class HomeController : Controller {
    /// <summary>
    /// 列出文件列表
    /// </summary>
    /// <param name="dir">文件夹路径，以 '/' 开头</param>
    /// <returns></returns>
    [HttpGet("list")]
    public JsonResponse<IEnumerable<FileVO>> ListFiles([FromQuery] string dir = "/") {
        dir = PathUtils.Normalize(dir);
        // 非法访问
        if (!PathUtils.CheckPathRange(dir)) {
            return JsonResponse<IEnumerable<FileVO>>.Forbidden;
        }
        return JsonResponse<IEnumerable<FileVO>>.Success with {
            Data = FileService.ListFiles(dir)
        };
    }

    /// <summary>
    /// 主页
    /// </summary>
    /// <returns></returns>
    public IActionResult Index() {
        return View("Views/Index.cshtml");
    }

    /// <summary>
    /// favicon
    /// </summary>
    /// <returns></returns>
    [HttpGet("favicon.ico")]
    public FileResult Favicon() => File(System.IO.File.OpenRead("favicon.ico"), "image/x-icon");

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="path">文件夹路径，以 '/' 开头</param>
    /// <returns></returns>
    [HttpGet("/download")]
    public FileResult? Donwload([FromQuery] string path) {
        path = PathUtils.Normalize(path);
        // 非法访问
        if (!PathUtils.CheckPathRange(path)) {
            return null;
        }
        var filePath = PathUtils.GetAbsolutePath(path);
        // 文件不存在
        if (!System.IO.File.Exists(filePath)) {
            return null;
        }
        return File(
            System.IO.File.OpenRead(filePath),
            "application/octet-stream",
            Path.GetFileName(filePath)
        );
    }

    /// <summary>
    /// 心跳测试
    /// </summary>
    /// <returns></returns>
    [HttpGet("/heartbeat")]
    public JsonResponse HeartBeat() => JsonResponse.Success;
}
