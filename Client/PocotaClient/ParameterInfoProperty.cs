using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public class ParameterInfoProperty(ParameterInfo info) : Property(info.Name!, info.ParameterType)
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
}
