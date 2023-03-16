namespace CommonUtil.Model;

public class NavigationRequestArgs : EventArgs {
    public Type ViewType { get; set; }
    public object? Data { get; set; }

    public NavigationRequestArgs(Type viewType) {
        ViewType = viewType;
    }

    public NavigationRequestArgs(Type viewType, object? data) : this(viewType) {
        Data = data;
    }
}
