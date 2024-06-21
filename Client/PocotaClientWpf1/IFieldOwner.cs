namespace Net.Leksi.Pocota.Client;

public interface IFieldOwner
{
    Field? Field { get; set; }
    object? Target { get; set; }
    string? PropertyName { get; set; }
    FieldOwnerCore? FieldOwnerCore { get; }
    void OnFieldAssigned();
}
