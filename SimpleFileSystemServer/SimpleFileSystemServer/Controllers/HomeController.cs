using CommonTools.Utils;
using Microsoft.AspNetCore.Mvc;
using SimpleFileSystemServer.Service;
using System.Text.RegularExpressions;

namespace SimpleFileSystemServer.Controllers;

[Route("")]
public class HomeController : Controller {
    private static readonly Regex InvalidFileNameCharsRegex = new($@"[{string.Join("", Path.GetInvalidFileNameChars())}]");

    /// <summary>
    /// 列出文件列表
    /// </summary>
    /// <param name="dir">文件夹路径，以 '/' 开头</param>
    /// <returns></returns>
    [HttpGet("/list")]
    public JsonResponse<IEnumerable<FileVO>> ListFiles([FromQuery] string dir = "/") {
        dir = PathUtils.Normalize(dir);
        // 非法访问
        if (!PathUtils.CheckPathRange(dir)) {
            return JsonResponse<IEnumerable<FileVO>>.Forbidden();
        }
        return JsonResponse<IEnumerable<FileVO>>.Success() with {
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
    /// 下载文件
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    [HttpPost("/upload")]
    [RequestFormLimits(MultipartBodyLengthLimit = ConstantUtils.OneGbSize << 6)]
    [RequestSizeLimit(ConstantUtils.OneGbSize << 6)]
    public async Task<JsonResponse> UploadFilesAsync([FromForm(Name = "dir")] string dir, [FromForm(Name = "file")] IEnumerable<IFormFile> files) {
        dir = PathUtils.Normalize(dir);
        // Invalid path
        if (!PathUtils.CheckPathRange(dir)) {
            return JsonResponse.Error;
        }

        var absDirPath = PathUtils.GetAbsolutePath(dir);
        foreach (var formFile in files) {
            // 检查文件名合法性
            var fileName = InvalidFileNameCharsRegex.Replace(formFile.FileName, "");
            var filePath = Path.Combine(absDirPath, fileName);
            // 检查文件是否重复
            filePath = CommonUtils.GetUniqueFileNameFor(filePath);
            await Console.Out.WriteLineAsync(filePath);
            using var stream = System.IO.File.OpenWrite(filePath);
            await TaskUtils.TryAsync(() => formFile.CopyToAsync(stream));
        }
        return JsonResponse.Success;
    }

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
