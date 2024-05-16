﻿using System.ComponentModel;
using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public abstract class Property(string name, Type type) : INotifyPropertyChanged
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
    public string Name { get; private init; } = name;
    public Type Type { get; private init; } = type;
    public abstract object? Value { get; set; }
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
