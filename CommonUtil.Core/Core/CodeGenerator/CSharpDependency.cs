namespace CommonUtil.Core;

public static class CSharpDependencyGenerator {
    private const string Indent = "    ";
    private const string DoubleIndent = "        ";

    public static string CreateTemplate(IEnumerable<TypeInfo> types) {
        if (!types.Any()) {
            return string.Empty;
        }
        string className = types.First().ClassName;
        #region 普通对象
        var poSb = new StringBuilder();
        poSb.AppendLine($"public class {className}PO {{");
        foreach (TypeInfo type in types) {
            poSb.AppendLine($"{Indent}/// <summary>");
            poSb.AppendLine($"{Indent}/// {type.Comment}");
            poSb.AppendLine($"{Indent}/// </summary>");
            poSb.AppendLine($@"{Indent}public {type.Type} {type.Name} {{get; set;}} = {type.Value};");
        }
        poSb.AppendLine("}");
        string poClass = poSb.ToString();
        #endregion

        #region DO对象
        var propertySb = new StringBuilder();
        var staticSb = new StringBuilder();
        foreach (var type in types) {
            propertySb.AppendLine($"{Indent}/// <summary>");
            propertySb.AppendLine($"{Indent}/// {type.Comment}");
            propertySb.AppendLine($"{Indent}/// </summary>");
            propertySb.AppendLine($"{Indent}public {type.Type} {type.Name} {{")
                .AppendLine($"{DoubleIndent}get {{ return ({type.Type})GetValue({type.Name}Property); }}")
                .AppendLine($"{DoubleIndent}set {{ SetValue({type.Name}Property, value); }}")
                .AppendLine($"{Indent}}}");
            staticSb.AppendLine($"{Indent}public static readonly DependencyProperty {type.Name}Property = DependencyProperty.Register(\"{type.Name}\", typeof({type.Type}), typeof({className}), new PropertyMetadata({type.Value}));");
        }
        string doClass = new StringBuilder($"public class {className} : DependencyObject {{\n")
            .AppendLine(staticSb.ToString())
            .Append(propertySb)
            .AppendLine("}")
            .ToString();
        #endregion
        return poClass + "\n" + doClass;
    }
}

public record TypeInfo {
    public TypeInfo() {

    }

    public TypeInfo(string name, string className, string type, string value, string comment) {
        Name = name;
        Type = type;
        ClassName = className;
        Value = value;
        Comment = comment;
    }

    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}
