﻿using System.ComponentModel;
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
                NotifyPropertyChanged();
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
        else if(info is Property property)
        {
            result = property;
        }
        if(result is { })
        {
            if (
                result.Type.IsGenericType 
                && (
                    typeof(IList<>).MakeGenericType([result.Type.GetGenericArguments()[0]]).IsAssignableFrom(result.Type)
                    || typeof(ICollection<>).MakeGenericType([result.Type.GetGenericArguments()[0]]).IsAssignableFrom(result.Type)
                )
            )
            {
                result = new ListProperty(result);
            }
        }
        return result;
    }
    public void NotifyPropertyChanged()
    {
        _propertyChanged?.Invoke(this, _propertyChangedEventArgs);
    }
}
