namespace Net.Leksi.Pocota.Tool;

internal class PropertyModel
{
    internal string Name { get; set; } = null!;
    internal string TypeName { get; set; } = null!;
    internal List<string> Attributes { get; private init; } = [];
    internal bool IsNullable { get; set; }
    internal bool IsCollection { get; set; }
    internal string? ItemTypeName { get; set; } = null;
}
