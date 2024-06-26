﻿using System.ComponentModel;
using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public class Property : INotifyPropertyChanged
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
    private bool _isSetReadonly = false;
    protected object? _value;

    public string Name { get; private init; }
    public Type Type { get; private init; }
    public bool IsSetReadOnly 
    { 
        get => _isSetReadonly;
        set
        {
            if(_isSetReadonly != value)
            {
                _isSetReadonly = value;
                NotifyPropertyChanged();
            }
        }
    }
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
    public virtual bool IsReadonly => IsSetReadOnly;
    public virtual bool IsNullable => false;
    
    public Property(string name, Type type)
    {
        Name = name;
        Type = type;
        _value = type.IsClass || type.IsInterface || IsNullable ? null : Activator.CreateInstance(type);
    }
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
