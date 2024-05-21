using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client;
public partial class EditObject : Window, IEditWindow, IWindowLauncher
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    private readonly Dictionary<string, WeakReference<EditObject>> _editWindows = [];
    private Property? _property;
    private Window? _launchedBy;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    public ObservableCollection<Property> Properties { get; private init; } = [];
    public CollectionViewSource PropertiesViewSource { get; private init; } = new();
    public bool IsReadonly { get; private set; }
    public bool KeysOnly { get; internal set; }
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
                if (PocotaContext.IsEntityType(_property.Type))
                {
                    foreach (Property prop in ((IPocotaEntity)_property.Value!).Properties)
                    {
                        Properties.Add(Property.Create(prop)!);
                    }
                }
                else
                {
                    foreach (PropertyInfo pi in _property.Type.GetProperties())
                    {
                        Property prop = Property.Create(pi, _property.Value)!;
                        Properties.Add(prop);
                    }
                }
            }
        }
    }
    public EditWindowCore EditWindowCore { get; private init; }
    public EditObject(string path, Type type, bool isReadonly = false)
    {
        PropertiesViewSource.Source = Properties;
        IsReadonly = false;
        _services = (IServiceProvider)Application.Current.Resources[ServiceProvider];
        _context = _services.GetRequiredService<PocotaContext>();
        Windows = _services.GetRequiredService<WindowsList>();
        EditWindowCore = new EditWindowCore(path, type);
        Launcher = new EditWindowLauncher(EditWindowCore.Path, this);
        InitializeComponent();
        CalcColumnsWidth(PropertiesView.ActualWidth);
        Windows.Touch();
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
