namespace Net.Leksi.Pocota.Tool;

internal class MethodModel
{
    internal string Name { get; set; } = null!;
    internal string ReturnTypeName { get; set; } = null!;
    internal List<ParameterModel> Parameters { get; private init; } = [];
    internal List<string> Attributes { get; private init; } = [];
    internal bool IsEnumeration { get; set; }
    internal string? ReturnItemTypeName { get; set; } = null!;
}
