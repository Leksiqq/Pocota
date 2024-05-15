using System.ComponentModel;

namespace Net.Leksi.Pocota.Client;
public abstract class Property(string name, Type type) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public string Name { get; private init; } = name;
    public Type Type { get; private init; } = type;
    public abstract object? Value { get; set; }
    public virtual bool IsReadonly => false;
    protected void OnPropertyChanged()
    {
        if (PropertyChanged is { })
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }
}
