using ModernWpf.Controls;

namespace CommonUtil.Utils;

public static class MiscUtils {
    /// <summary>
    /// 转换浮点数为整数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void NumberBoxDoubleToIntLostFocusHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 浮点数转整数
        if (sender is NumberBox numberBox) {
            TaskUtils.Try(() => numberBox.Value = (int)numberBox.Value);
        }
    }
}
