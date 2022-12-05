namespace CommonUtil.Utils;

public static class ProcessStatusUtils {
    /// <summary>
    /// 在任务完成后时，更新 ProcessStatus Status，如果是 Successful 则同时设置 Process 为 1
    /// </summary>
    /// <param name="status"></param>
    /// <param name="result"></param>
    /// <remarks>可在任意线程调用</remarks>
    public static void UpdateProcessStatusWhenCompleted(FileProcessStatus status, ProcessResult result) {
        UIUtils.RunOnUIThread(() => {
            status.Status = result;
            if (result == ProcessResult.Successful) {
                status.Process = 1;
            }
        });
    }

    /// <summary>
    /// 在任务完成后时，更新 ProcessStatus，如果任务没有取消则设置 Status = Successful,
    /// Process = 1, FileSize = fileSize，否则设置 Status = Interrupted
    /// </summary>
    /// <param name="status"></param>
    /// <param name="token"></param>
    /// <param name="fileSize"></param>
    /// <remarks>可在任意线程调用</remarks>
    public static void UpdateProcessStatusWhenCompleted(FileProcessStatus status, CancellationToken token, long fileSize) {
        UIUtils.RunOnUIThread(() => {
            if (token.IsCancellationRequested) {
                status.Status = ProcessResult.Interrupted;
            } else {
                status.Status = ProcessResult.Successful;
                status.Process = 1;
                status.FileSize = fileSize;
            }
        });
    }

    /// <summary>
    /// 在任务完成后时，更新 ProcessStatus，如果任务没有取消则设置 Status = Successful, 
    /// Process = 1，否则设置 Status = Interrupted
    /// </summary>
    /// <param name="status"></param>
    /// <param name="token"></param>
    public static void UpdateProcessStatusWhenCompleted(FileProcessStatus status, CancellationToken token) {
        UIUtils.RunOnUIThread(() => {
            if (token.IsCancellationRequested) {
                status.Status = ProcessResult.Interrupted;
            } else {
                status.Status = ProcessResult.Successful;
                status.Process = 1;
            }
        });
    }

    /// <summary>
    /// 更新 ProcessStatus Process
    /// </summary>
    /// <param name="status"></param>
    /// <param name="process"></param>
    /// <remarks>可在任意线程调用</remarks>
    public static void UpdateProcessStatus(FileProcessStatus status, double process) {
        UIUtils.RunOnUIThread(() => status.Process = process);
    }
}