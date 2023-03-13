namespace CommonUtil.Utils;

/// <summary>
/// 文件处理工具
/// </summary>
public static class FileProcessUtils {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 处理多文件任务
    /// </summary>
    /// <param name="sourceFilenames">源文件列表</param>
    /// <param name="saveDirectoryDialog">保存文件夹 Dialog</param>
    /// <param name="source">CancellationTokenSource</param>
    /// <param name="fileProcessStatuses">进度列表</param>
    /// <param name="processFunc">处理方法，参数列表为 (inputFile, outputFile, FileProcessStatus)</param>
    /// <param name="parentWindow">所属 Window，默认为 Application.Current.MainWindow</param>
    /// <param name="log">日志，默认为 <see cref="Logger"/></param>
    /// <returns>Task</returns>
    [NoException]
    public static async Task ProcessMultiFilesAsync(
         ICollection<string> sourceFilenames,
         VistaFolderBrowserDialog saveDirectoryDialog,
         CancellationTokenSource source,
         ObservableCollection<FileProcessStatus> fileProcessStatuses,
         Action<string, string, FileProcessStatus> processFunc,
         Window? parentWindow = null,
         Logger? log = null
    ) {
        parentWindow ??= Application.Current.MainWindow;
        var dispatcher = parentWindow.Dispatcher;
        log ??= Logger;
        // 选择保存目录
        if (saveDirectoryDialog.ShowDialog(parentWindow) != true) {
            return;
        }
        var saveDirectory = saveDirectoryDialog.SelectedPath;
        var tasks = new List<Task>();
        int chunkSize = (int)Math.Ceiling(sourceFilenames.Count / (double)Global.ConcurrentTaskCount);
        // 分配任务运行
        foreach (var files in sourceFilenames.Chunk(chunkSize)) {
            tasks.Add(Task.Run(() => {
                foreach (var file in files) {
                    // 任务取消
                    if (source.IsCancellationRequested) {
                        log.Info("任务取消");
                        return;
                    }
                    var outputFile = Path.Combine(saveDirectory, Path.GetFileName(file));
                    var status = dispatcher.Invoke(() => {
                        var status = new FileProcessStatus() {
                            FileName = outputFile,
                            Status = ProcessResult.Processing
                        };
                        fileProcessStatuses.Add(status);
                        return status;
                    });

                    try {
                        processFunc(file, outputFile, status);
                        ProcessStatusUtils.UpdateProcessStatusWhenCompleted(
                            status,
                            source.Token,
                            new FileInfo(outputFile).Length
                        );
                    } catch (Exception error) {
                        log.Error(error);
                        ProcessStatusUtils.UpdateProcessStatusWhenCompleted(status, ProcessResult.Failed);
                    }
                }
            }, source.Token));
        }
        await Task.WhenAll(tasks);
    }

    /// <param name="processFunc">处理方法</param>
    /// <inheritdoc cref="ProcessMultiFilesAsync(ICollection{string}, VistaFolderBrowserDialog, CancellationTokenSource, ObservableCollection{FileProcessStatus}, Action{string, string, FileProcessStatus}, Window?, Logger?)"/>
    public static async Task ProcessMultiFilesAsync(
        ICollection<string> sourceFilenames,
        VistaFolderBrowserDialog saveDirectoryDialog,
        CancellationTokenSource source,
        ObservableCollection<FileProcessStatus> fileProcessStatuses,
        CommonFileProcess processFunc,
        Window? parentWindow = null,
        Logger? log = null
     ) {
        await ProcessMultiFilesAsync(
            sourceFilenames,
            saveDirectoryDialog,
            source,
            fileProcessStatuses,
            (inputFile, outputFile, status) => processFunc(
                inputFile,
                outputFile,
                source.Token,
                proc => ThrottleUtils.Throttle(status, () => {
                    ProcessStatusUtils.UpdateProcessStatus(status, proc);
                })
            ),
            parentWindow,
            log
        );
    }

    /// <inheritdoc cref="ProcessMultiFilesAsync(ICollection{string}, VistaFolderBrowserDialog, CancellationTokenSource, ObservableCollection{FileProcessStatus}, CommonFileProcess, Window?, Logger?)"/>
    public static async Task ProcessMultiFilesAsync<T>(
        ICollection<string> sourceFilenames,
        VistaFolderBrowserDialog saveDirectoryDialog,
        CancellationTokenSource source,
        ObservableCollection<FileProcessStatus> fileProcessStatuses,
        CommonFileProcess<T> processFunc,
        Window? parentWindow = null,
        Logger? log = null
     ) {
        await ProcessMultiFilesAsync(
            sourceFilenames,
            saveDirectoryDialog,
            source,
            fileProcessStatuses,
            (inputFile, outputFile, token, callback) => {
                processFunc(inputFile, outputFile, token, callback);
            },
            parentWindow,
            log
        );
    }

    /// <summary>
    /// 处理单个文件
    /// </summary>
    /// <param name="sourceFilename">源文件</param>
    /// <param name="saveFileDialog">保存文件 Dialog</param>
    /// <param name="source">CancellationTokenSource</param>
    /// <param name="fileProcessStatuses">进度列表</param>
    /// <param name="processFunc">处理方法，参数列表为 (inputFile, outputFile, FileProcessStatus)</param>
    /// <param name="log">日志，默认为 <see cref="Logger"/></param>
    /// <returns>Task</returns>
    [NoException]
    public static async Task ProcessOneFileAsync(
        string sourceFilename,
        SaveFileDialog saveFileDialog,
        CancellationTokenSource source,
        ObservableCollection<FileProcessStatus> fileProcessStatuses,
        Action<string, string, FileProcessStatus> processFunc,
        Logger? log = null
    ) {
        log ??= Logger;
        saveFileDialog.FileName = Path.GetFileName(sourceFilename);
        if (saveFileDialog.ShowDialog() != true) {
            return;
        }
        var saveFilename = saveFileDialog.FileName;
        var status = new FileProcessStatus() {
            FileName = saveFilename,
            Status = ProcessResult.Processing,
        };
        fileProcessStatuses.Add(status);

        // 处理
        try {
            await Task.Run(
                () => processFunc(sourceFilename, saveFilename, status),
                source.Token
            );
            // 任务取消
            if (source.IsCancellationRequested) {
                status.Status = ProcessResult.Interrupted;
            }
            // 通知
            else {
                status.Status = ProcessResult.Successful;
                status.Process = 1;
                status.FileSize = new FileInfo(saveFilename).Length;
                UIUtils.NotificationOpenFileInExplorerAsync(saveFilename, title: "处理成功");
            }
        } catch (IOException error) {
            status.Status = ProcessResult.Failed;
            log.Error(error);
            MessageBoxUtils.Error("文件读取或保存失败");
        } catch (Exception error) {
            status.Status = ProcessResult.Failed;
            log.Error(error);
            MessageBoxUtils.Error("处理失败");
        }
    }

    /// <param name="processFunc">处理方法</param>
    /// <inheritdoc cref="ProcessOneFileAsync(string, SaveFileDialog, CancellationTokenSource, ObservableCollection{FileProcessStatus}, Action{string, string, FileProcessStatus}, Logger?)"/>
    public static async Task ProcessOneFileAsync(
        string sourceFilename,
        SaveFileDialog saveFileDialog,
        CancellationTokenSource source,
        ObservableCollection<FileProcessStatus> fileProcessStatuses,
        CommonFileProcess processFunc,
        Logger? log = null
    ) {
        await ProcessOneFileAsync(
            sourceFilename,
            saveFileDialog,
            source,
            fileProcessStatuses,
            (inputFile, outputFile, status) => processFunc(
                inputFile,
                outputFile,
                source.Token,
                proc => ThrottleUtils.Throttle(status, () => {
                    ProcessStatusUtils.UpdateProcessStatus(status, proc);
                })
            ),
            log
        );
    }

    /// <inheritdoc cref="ProcessOneFileAsync(string, SaveFileDialog, CancellationTokenSource, ObservableCollection{FileProcessStatus}, CommonFileProcess, Logger?)"/>
    public static async Task ProcessOneFileAsync<T>(
        string sourceFilename,
        SaveFileDialog saveFileDialog,
        CancellationTokenSource source,
        ObservableCollection<FileProcessStatus> fileProcessStatuses,
        CommonFileProcess<T> processFunc,
        Logger? log = null
    ) {
        await ProcessOneFileAsync(
            sourceFilename,
            saveFileDialog,
            source,
            fileProcessStatuses,
            (inputFile, outputFile, token, callback) => {
                processFunc(inputFile, outputFile, token, callback);
            },
            log
        );
    }
}
