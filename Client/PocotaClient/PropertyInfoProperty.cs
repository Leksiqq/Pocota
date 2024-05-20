using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public class PropertyInfoProperty: Property
{
    private readonly PropertyInfo _info;
    private readonly object _obj;
    public override object? Value 
    { 
        get => _info.GetValue(_obj); 
        set
        {
            if(_info.GetValue(_obj) != value)
            {
                _info.SetValue(_obj, value);
                NotifyPropertyChanged();
            }
        }
    }
    public override bool IsReadonly
    {
        get
        {
            if(!_info.CanWrite)
            {
                return true;
            }
            return _info.SetMethod!.ReturnParameter
                .GetRequiredCustomModifiers().Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
        }
    }
    internal PropertyInfoProperty(PropertyInfo info, object obj) : base(info.Name, info.PropertyType) 
    {
        _info = info;
        _obj = obj;
    }
}
