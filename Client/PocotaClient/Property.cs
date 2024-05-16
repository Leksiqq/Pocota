using System.ComponentModel;
using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public class Property(string name, Type type) : INotifyPropertyChanged
{
    private event PropertyChangedEventHandler? _propertyChanged;
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add
        {
            _propertyChanged += value;
        }
        remove
        {
            _propertyChanged -= value;
        }
    }
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(nameof(Value));
    protected object? _value;

    public string Name { get; private init; } = name;
    public Type Type { get; private init; } = type;
    public virtual object? Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }
    public virtual bool IsReadonly => false;
    public static Property? Create(object info, object? value = null)
    {
        Property? result = null;
        if (info is PropertyInfo pi && value is { })
        {
            result = new PropertyInfoProperty(pi, value);
        }
        else if (info is ParameterInfo par)
        {
            result = new ParameterInfoProperty(par);
        }
        else if(info is Type type)
        {
            result = new Property(string.Empty, type);
        }
        if(result is { })
        {
            if (
                result.Type.IsGenericType && typeof(IList<>)
                        .MakeGenericType([result.Type.GetGenericArguments()[0]]).IsAssignableFrom(result.Type)
            )
            {
                result = new ListProperty(result);
            }
        }
        return result;
    }
    protected void OnPropertyChanged()
    {
        if (_propertyChanged is { })
        {
            _propertyChanged.Invoke(this, _propertyChangedEventArgs);
        }
    }
}
