using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public class PropertyInfoProperty: Property
{
    private readonly PropertyInfo _info;
    private readonly object _obj;
    private readonly bool _isNullable;
    public override object? Value 
    { 
        get => _info.GetValue(_obj); 
        set
        {
            if(_info.GetValue(_obj) != value)
            {
                Console.WriteLine($"{GetHashCode()}, {value}");
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
    public override bool IsNullable => _isNullable;
    public override object? Declarator => _info.DeclaringType;
    internal PropertyInfoProperty(PropertyInfo info, object obj) : base(info.Name, info.PropertyType) 
    {
        _info = info;
        _obj = obj;
        NullabilityInfoContext nic = new();
        _isNullable = nic.Create(info).ReadState is NullabilityState.Nullable;
    }
}
