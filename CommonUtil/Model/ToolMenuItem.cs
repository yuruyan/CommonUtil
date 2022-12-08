namespace CommonUtil.Model;

public readonly record struct ToolMenuItem {
    public string Name { get; init; }
    public string ImagePath { get; init; }
    public Type ClassType { get; init; }

    public ToolMenuItem(string name, string imagePath, Type classType) {
        Name = name;
        ImagePath = imagePath;
        ClassType = classType;
    }
}
