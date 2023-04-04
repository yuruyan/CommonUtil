namespace CommonUtil.View;

public static class RandomGeneratorHelper {
    public const string RandomGeneratorDescriptionWidthKey = "RandomGenerator_DescriptionWidth";
    public const string DescriptionHeaderTag = "DescriptionHeader";
    private static readonly ResourceDictionary OtherResources = App.Current.Resources.MergedDictionaries.FindResource("/CommonUtil;component/Resources/ResourceDictionary/OtherResources.xaml")!;

    /// <summary>
    /// 更新 DescriptionWidth
    /// </summary>
    /// <param name="width"></param>
    public static void UpdateDescriptionWidth(double width) {
        OtherResources[RandomGeneratorDescriptionWidthKey] = width;
    }

    /// <summary>
    /// 获取 DescriptionWidth
    /// </summary>
    public static double DescriptionWidth => (double)OtherResources[RandomGeneratorDescriptionWidthKey];

    /// <summary>
    /// 查找 <see cref="FrameworkElement.Tag"/> 为 <see cref="DescriptionHeaderTag"/> 的元素
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public static IList<FrameworkElement> FindDescriptionHeaders(DependencyObject view) {
        return view.FindDescendantsBy(o => {
            return o is FrameworkElement element && element.Tag?.ToString() == DescriptionHeaderTag;
        }).OfType<FrameworkElement>()
            .ToList();
    }

    /// <summary>
    /// 获取标记了 <see cref="DescriptionHeaderTag"/> 元素的 MaxWidth
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public static double GetDescriptionHeadersMaxWidth(IList<FrameworkElement> elements) {
        return elements.Count == 0 ? 0 : elements.Max(e => e.ActualWidth);
    }
}
