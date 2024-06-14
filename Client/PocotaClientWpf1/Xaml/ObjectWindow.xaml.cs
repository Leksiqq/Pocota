﻿using Net.Leksi.Pocota.Client.UserControls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
namespace Net.Leksi.Pocota.Client;
public partial class ObjectWindow : Window, IWindowWithCore, IServiceRelated, INotifyPropertyChanged, IEditWindow, IValueConverter
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private Property? _property;
    private IInputElement? _currentInput = null;
    public WindowCore Core { get; private init; }
    public string ServiceKey { get; private init; }
    public Property? Property 
    { 
        get => _property; 
        internal set
        {
            if (_property != value)
            {
                if (_property is { })
                {
                    Properties.Clear();
                }
                _property = value;
                if (_property is { })
                {
                    if (_property.Value is IEntityOwner eo)
                    {
                    }
                    else if (_property.Value is {})
                    {
                        foreach(PropertyInfo pi in _property.Type.GetProperties())
                        {
                            Properties.Add(Property.Create(pi, _property.Value)!);
                        }
                    }
                }
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
        } 
    }
    public ObservableCollection<Property> Properties { get; private init; } = [];
    public string ObjectTitle => $"{(Core.Launcher?.Owner is IEditWindow ew ? $"{ew.ObjectTitle}/" : string.Empty)}{Property?.Name ?? string.Empty}";

    public ObjectWindow(string serviceKey, Window owner)
    {
        Core = new WindowCore(this);
        ServiceKey = serviceKey;
        Core.Launcher = (owner as IWindowWithCore)?.Core;
        InitializeComponent();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine(value);
        return value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    private void ObjectEditor_CurrentInputChanged(object sender, EventArgs e)
    {
        if (sender is ObjectEditor oe) 
        {
            InsertInputMode.Text = (string?)oe.Convert(oe.CurrentInput?.IsInsertMode, typeof(string), "InsertInputMode", CultureInfo.CurrentCulture);
        }
    }
}
