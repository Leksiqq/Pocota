using System.ComponentModel;

namespace Net.Leksi.Pocota.Client;

public class MethodParameter: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public string Name { get; private init; }
    public Type Type { get; private init; }
    public object? Value { get; set; } = null;
    internal MethodParameter(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}
