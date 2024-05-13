using System.ComponentModel;

namespace Net.Leksi.Pocota.Client;

public class NamedValue: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private object? _value;
    public string Name { get; private init; }
    public Type Type { get; private init; }
    public object? Value 
    {
        get => _value;
        set
        {
            _value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
    public bool IsReadonly { get; internal set; }
    internal NamedValue(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}
