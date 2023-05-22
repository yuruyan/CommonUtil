namespace CommonUtil.Model;

public record NavigationItemInfo {
    public string? Name { get; init; }
    public string Content { get; init; }
    /// <summary>
    /// If not specified, <see cref="ToolTip"/> will be the same as <see cref="Content"/>
    /// </summary>
    public string? ToolTip { get; init; }
    public string? Icon { get; init; }
    public string? IconColor { get; init; }
    /// <summary>
    /// If specified, <see cref="Icon"/> is ignored
    /// </summary>
    public Uri? IconImage { get; init; }
    public bool ShowAsMonochrome { get; init; }

    public NavigationItemInfo(string content) {
        Content = content;
    }
}
