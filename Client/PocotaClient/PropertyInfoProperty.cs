using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public class PropertyInfoProperty : Property
{
    private readonly object _obj;
    private readonly PropertyInfo _info;
    public override object? Value 
    { 
        get => _info.GetValue(_obj); 
        set
        {
            if(_info.GetValue(_obj) != value)
            {
                _info.SetValue(_obj, value);
                OnPropertyChanged();
            }
        }
    }
    public PropertyInfoProperty(PropertyInfo info, object obj): base(info.Name, info.PropertyType)
    {
        _info = info;
        _obj = obj;
    }
}
