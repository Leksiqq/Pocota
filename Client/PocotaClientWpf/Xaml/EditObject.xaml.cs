﻿using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client;
public partial class EditObject : Window, IEditWindow
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IServiceProvider _services;
    private readonly Dictionary<string, WeakReference<EditObject>> _editWindows = [];
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private Property? _property;
    private Window? _launchedBy;
    private string? _serviceKey = null;
    private PocotaContext? _context = null;
    public ObservableCollection<Property> Properties { get; private init; } = [];
    public CollectionViewSource PropertiesViewSource { get; private init; } = new();
    public bool IsReadonly { get; private set; }
    public bool KeysOnly { get; set; }
    public string ServiceKey 
    {
        get => _serviceKey ?? string.Empty; 
        set
        {
            if(_serviceKey is null && value != _serviceKey)
            {
                _serviceKey = value;
                _context = _services.GetRequiredKeyedService<PocotaContext>(ServiceKey);
            }
        }
    }
    public WindowsList Windows { get; private init; }
    public Window? LaunchedBy 
    { 
        get => _launchedBy; 
        set
        {
            if(_launchedBy != value)
            {
                _launchedBy = value;
                NotifyPropertyChanged();
            }
        }
    }
    public EditWindowLauncher Launcher { get; private init; }
    public Property? Property
    {  
        get => _property; 
        set
        {
            if(_property != value && value is { })
            {
                _property = value;
                Properties.Clear();
                if (typeof(IEntityOwner).IsAssignableFrom(_property.Type))
                {
                    foreach (Property prop in ((IEntityOwner)_property.Value!).Entity.Properties)
                    {
                        Property prop1 = Property.Create(prop)!;
                        if(prop1 is EntityProperty ep && ep.Access is not Contract.AccessKind.Key)
                        {
                        }
                        Properties.Add(prop1);
                    }
                }
                else
                {

                    foreach (PropertyInfo pi in _property.Type.GetProperties())
                    {
                        Properties.Add(Property.Create(pi, _property.Value)!);
                    }
                }
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
        }
    }
    public string? PropertyHash => $"{Property?.GetType()}:{Property?.GetHashCode()}";
    public EditWindowCore EditWindowCore { get; private init; }
    public EditObject(string path, Type type, bool isReadonly = false)
    {
        PropertiesViewSource.Source = Properties;
        IsReadonly = false;
        _services = (IServiceProvider)Application.Current.Resources[ServiceProvider];
        Windows = _services.GetRequiredService<WindowsList>();
        EditWindowCore = new EditWindowCore(path, type);
        Launcher = new EditWindowLauncher(EditWindowCore.Path, this);
        InitializeComponent();
        CalcColumnsWidth(PropertiesView.ActualWidth);
        Windows.Touch();
    }
    protected override void OnClosed(EventArgs e)
    {
        Windows.Touch();
        _launchedBy?.Activate();
        base.OnClosed(e);
    }
    private void NotifyPropertyChanged()
    {
        PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
    }
    private void CalcColumnsWidth(double width)
    {
        PropertyValueColumn.Width = width * 0.8 - PropertyNameColumn.ActualWidth;
    }
    private void PropertiesView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            CalcColumnsWidth(PropertiesView.ActualWidth);
        }
    }
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if(LaunchedBy is { })
        {
            LaunchedBy.Focus();
        }
    }
}
