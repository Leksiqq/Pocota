using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Net.Leksi.Pocota.Tool;

public class ClassModel: PageModel
{
    private string _namespace = null!;
    internal string NamespaceValue
    {
        get => _namespace;
        set
        {
            _namespace = value;
            Usings.Remove(_namespace);
        }
    }
    internal string ClassName { get; set; } = null!;
    internal string Contract { get; set; } = null!;
    internal HashSet<string> Usings { get; private init; } = [];
    internal HashSet<string> Inheritances { get; private init; } = [];
    internal List<PropertyModel> Properties { get; private init; } = [];
    internal List<MethodModel> Methods { get; private init; } = [];
    internal List<string> Attributes { get; private init; } = [];
    internal void AddUsing(Type type)
    {
        if (!string.IsNullOrEmpty(type.Namespace) && type.Namespace != NamespaceValue)
        {
            Usings.Add(type.Namespace);
        }
    }
    internal void AddUsing(string ns)
    {
        if (!string.IsNullOrEmpty(ns) && ns != NamespaceValue)
        {
            Usings.Add(ns);
        }
    }
    internal void AddInheritance(Type type)
    {
        AddUsing(type);
        Inheritances.Add(Util.BuildTypeName(type));
    }
    internal void AddInheritance(string typeName)
    {
        Inheritances.Add(typeName);
    }
}
