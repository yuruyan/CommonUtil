namespace CommonUtil.Model;

public class AutomationItem {
    public string Name { get; }
    public string Icon { get; }
    public bool IsFolder { get; }
    public IReadOnlyList<AutomationItem> Children { get; init; } = new List<AutomationItem>();

    public AutomationItem(string name, string icon, bool isFolder = false) {
        Name = name;
        Icon = icon;
        IsFolder = isFolder;
    }
}
