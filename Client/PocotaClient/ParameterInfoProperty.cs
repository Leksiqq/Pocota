using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public class ParameterInfoProperty: Property
{
    private object? _value;
    public override object? Value
    {
        get => _value;
        set
        {
            if(_value != value)
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }
    internal ParameterInfoProperty(ParameterInfo info) : base(info.Name!, info.ParameterType) { }
}
