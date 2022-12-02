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
    [HttpGet("{dir}")]
    public JsonResponse<IEnumerable<FileVO>> ListFiles(string dir = "/") {
        dir = "/" + (dir ?? "").Trim('\\', '/');
        // 非法访问
        if (!PathUtils.CheckPathRange(dir)) {
            return JsonResponse<IEnumerable<FileVO>>.Forbidden;
        }
        return JsonResponse<IEnumerable<FileVO>>.Success with {
            Data = FileService.ListFiles(dir)
        };
    }

    /// <summary>
    /// 主页，列出根目录文件列表
    /// </summary>
    /// <returns></returns>
    public JsonResponse<IEnumerable<FileVO>> Index() => ListFiles();

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="path">文件路径，以 '/' 开头</param>
    /// <returns></returns>
    [HttpGet("/download/{path}")]
    public FileResult? Donwload(string path) {
        // 非法访问
        if (!PathUtils.CheckPathRange(path)) {
            return null;
        }
        var filePath = PathUtils.GetAbsolutePath(path);
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
    public JsonResponse HeartBeat() {
        return new JsonResponse() { Code = 200, Message = "success" };
    }
}
