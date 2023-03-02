namespace CommonUtil.Model;

public class AutomationItem {
    public uint Id { get; }
    public string Name { get; }
    public string Icon { get; }
    public bool IsFolder { get; }
    public IReadOnlyList<AutomationItem> Children { get; init; } = new List<AutomationItem>();

    public AutomationItem(string name, string icon, uint id = 0, bool isFolder = false) {
        Id = id;
        Name = name;
        Icon = icon;
        IsFolder = isFolder;
    }
}
