namespace Net.Leksi.Pocota.Client;

public interface IFieldOwner
{
    IField? Field { get; set; }
    object? Target { get; set; }
    string? PropertyName { get; set; }
    void OnFieldAssigned();
}
