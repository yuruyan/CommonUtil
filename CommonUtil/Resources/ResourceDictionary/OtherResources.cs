namespace CommonUtil.Resources.ResourceDictionary;

public partial class OtherResources {
    /// <summary>
    /// 转换浮点数为整数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RandomGenerator_NumberBoxLostFocus(object sender, RoutedEventArgs e) {
        MiscUtils.NumberBoxDoubleToIntLostFocusHandler(sender, e);
    }
}
