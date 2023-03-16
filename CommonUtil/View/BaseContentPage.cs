namespace CommonUtil.View;

public class BaseContentPage : Page {
    public BaseContentPage() {
        SetBinding(MarginProperty, new Binding() {
            Source = new DynamicResourceExtension(ModernWpf.Controls.TitleBar.HeightKey)
        });
    }
}
