namespace Net.Leksi.Pocota.Tool;

internal class ParameterModel
{
    internal string Name { get; set; } = null!;
    internal string TypeName { get; set; } = null!;
    internal string? ItemTypeName { get; set; } = null!;
    internal bool IsNullable { get; set; } = false;
}
