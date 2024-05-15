﻿using System.Reflection;

namespace Net.Leksi.Pocota.Client;
public class PropertyInfoProperty(PropertyInfo info, object obj) : Property(info.Name, info.PropertyType)
{
    public override object? Value 
    { 
        get => info.GetValue(obj); 
        set
        {
            if(info.GetValue(obj) != value)
            {
                info.SetValue(obj, value);
                OnPropertyChanged();
            }
        }
    }
    public override bool IsReadonly => !info.CanWrite;
}
