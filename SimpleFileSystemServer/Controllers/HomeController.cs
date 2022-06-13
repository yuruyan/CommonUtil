using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleFileSystemServer.Service;

namespace SimpleFileSystemServer.Controllers;

[Route("")]
public class HomeController : Controller {
    private readonly FileService FileService = new();

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
}
