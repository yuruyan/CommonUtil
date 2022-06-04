using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtil.Core;

public class CSharpDependencyGenerator {
    public static string CreateTemplate(IEnumerable<TypeInfo> types) {
        if (!types.Any()) {
            return string.Empty;
        }
        string className = types.First().ClassName;
        var poSb = new StringBuilder();
        foreach (TypeInfo type in types) {
            poSb.Append($@"    public {type.Type} {type.Name} {{get; set;}} = {type.Value};").Append('\n');
        }
        string poClass = $@"public class {className}PO {{

{poSb}
}}";
        var doSb1 = new StringBuilder();
        var doSb2 = new StringBuilder();
        foreach (var type in types) {
            doSb1.Append($@"
    public {type.Type} {type.Name} {{
        get {{ return ({type.Type})GetValue({type.Name}Property); }}
        set {{ SetValue({type.Name}Property, value); }}
    }}");
            doSb2.Append($@"    public static readonly DependencyProperty {type.Name}Property = DependencyProperty.Register(""{type.Name}"", typeof({type.Type}), typeof({className}DO), new PropertyMetadata({type.Value}));").Append('\n');
        }

        string doClass = $@"public class {className}DO:DependencyObject {{
    {doSb1}

{doSb2}
}}";
        return poClass + "\n\n" + doClass;
    }
}

public class TypeInfo {
    public TypeInfo() {

    }

    public TypeInfo(string name, string className, string type, string value) {
        Name = name;
        Type = type;
        ClassName = className;
        Value = value;
    }

    public string Name { get; set; }
    public string Type { get; set; }
    public string ClassName { get; set; }
    public string Value { get; set; }

    public override string ToString() {
        return $"{{{nameof(Name)}={Name}, {nameof(Type)}={Type}, {nameof(ClassName)}={ClassName}, {nameof(Value)}={Value}}}";
    }
}
