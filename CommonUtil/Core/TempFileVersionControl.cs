using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using NLog;
using System;
using System.IO;

namespace CommonUtil.Core;

public class TempFileVersionControl : IDisposable {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 文件变化回调
    /// </summary>
    public Action<FileInfo> Callback { get; }
    /// <summary>
    /// 监听文件
    /// </summary>
    public string WatchFile { get; }
    /// <summary>
    /// 监听文件 FileInfo
    /// </summary>
    private FileInfo _watchFileInfo;
    /// <summary>
    /// Dispose
    /// </summary>
    private IDisposable _watchDispose;

    private TempFileVersionControl(string watchFile, Action<FileInfo> callback) {
        Callback = callback;
        WatchFile = watchFile;
        var value = Init();
        _watchFileInfo = value._watchFileInfo;
        _watchDispose = value._watchDispose;
    }

    public static TempFileVersionControl Watch(string watchFile, Action<FileInfo> callback) => new(watchFile, callback);

    /// <summary>
    /// 初始化
    /// </summary>
    /// <exception cref="FileNotFoundException">文件未找到</exception>
    private dynamic Init() {
        if (!File.Exists(WatchFile)) {
            throw new FileNotFoundException($"File '{WatchFile}' not found");
        }
        _watchFileInfo = new FileInfo(WatchFile);
        var pfp = new PhysicalFileProvider(_watchFileInfo.DirectoryName);
        _watchDispose = ChangeToken.OnChange(
             () => pfp.Watch(_watchFileInfo.Name),
             () => Callback(_watchFileInfo)
         );
        return new {
            _watchFileInfo,
            _watchDispose
        };
    }

    public void Dispose() => _watchDispose.Dispose();
}
