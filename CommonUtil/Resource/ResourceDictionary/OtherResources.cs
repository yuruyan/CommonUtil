using ModernWpf.Controls;

namespace CommonUtil.Resource.ResourceDictionary;

public partial class OtherResources {
    /// <summary>
    /// 转换浮点数为整数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RandomGenerator_NumberBoxLostFocus(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 浮点数转整数
        if (sender is NumberBox numberBox) {
            TaskUtils.Try(() => numberBox.Value = (int)numberBox.Value);
        }
    }
}
