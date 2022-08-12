using CommonUITools.Utils;
using ModernWpf.Controls;
using System.Windows;

namespace CommonUtil.View;

internal class RandomGeneratorUtil {
    /// <summary>
    /// 转换浮点数为整数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void NumberBoxDoubleToInt(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 浮点数转整数
        if (sender is NumberBox numberBox) {
            CommonUtils.Try(() => numberBox.Value = (int)numberBox.Value);
        }
    }
}
