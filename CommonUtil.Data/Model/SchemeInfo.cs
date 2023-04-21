namespace CommonUtil.Data.Model;

public record CodeColorizationSchemeInfo {
    public string Name { get; }
    public string[] FileTypes { get; }
    public string ResourceName { get; }

    public CodeColorizationSchemeInfo(string name, string[] fileTypes, string resourceName) {
        Name = name;
        FileTypes = fileTypes;
        ResourceName = resourceName;
    }
}
