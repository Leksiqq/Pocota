using System.ComponentModel;

namespace Net.Leksi.Pocota.Client;

public class MethodParameter: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private object? _value = "QWERT";
    public string Name { get; private init; }
    public Type Type { get; private init; }
    public object? Value 
    {
        get => _value;
        set
        {
            _value = value;
            Console.WriteLine(_value);
        }
    }
    internal MethodParameter(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}
