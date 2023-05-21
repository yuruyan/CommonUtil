using System.Windows.Shell;

namespace CommonUtil.Utils;

public static class TaskBarUtils {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Add recent menu to taskbar
    /// </summary>
    /// <param name="viewType"></param>
    public static void AddToTaskBarRecentList(Type viewType) {
        if (!DataSet.ToolMenuItems.TryGet(item => item.ClassType == viewType, out var menuItem)) {
            Logger.Info($"Cannot find ToolMenuItem for the type {viewType}");
            return;
        }
        JumpList.AddToRecentCategory(new JumpTask {
            ApplicationPath = Environment.ProcessPath,
            IconResourcePath = Path.Join(Global.MenuItemsDllDirectory, $"{menuItem.Id}.dll"),
            Arguments = menuItem.Id,
            Description = menuItem.Name,
            Title = menuItem.Name,
        });
    }
}
