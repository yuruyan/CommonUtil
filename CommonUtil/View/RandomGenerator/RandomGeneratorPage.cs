namespace CommonUtil.View;

public abstract class RandomGeneratorPage : Page, IGenerable<uint, IEnumerable<string>> {
    public RandomGeneratorPage() {
        Loaded += ViewLoadedHandler;
    }

    protected virtual void ViewLoadedHandler(object sender, RoutedEventArgs e) {
        var elements = RandomGeneratorHelper.FindDescriptionHeaders(this);
        elements.ForEach(e => {
            e.ClearValue(WidthProperty);
            e.UpdateLayout();
        });
        RandomGeneratorHelper.UpdateDescriptionWidth(RandomGeneratorHelper.GetDescriptionHeadersMaxWidth(elements));
        // Bind width
        elements.ForEach(e => {
            e.SetResourceReference(WidthProperty, RandomGeneratorHelper.RandomGeneratorDescriptionWidthKey);
        });
    }

    public abstract IEnumerable<string> Generate(uint count);
}
