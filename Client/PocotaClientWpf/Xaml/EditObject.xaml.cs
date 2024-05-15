using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client;
public partial class EditObject : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    private object? _value;
    public ObservableCollection<Property> Properties { get; private init; } = [];
    public bool IsReadonly { get; set; }
    public bool KeysOnly { get; set; }
    public WindowsList Windows { get; private init; }
    internal Window? Launcher { get; set; }
    public string ParameterName { get; set; }
    public string MethodName { get; set; }
    internal object? Value 
    {  
        get => _value; 
        set
        {
            if(_value != value && value is { })
            {
                _value = value;
                Properties.Clear();
                if (PocotaContext.IsEntityType(_value.GetType()))
                {

                }
                else
                {
                    foreach (PropertyInfo pi in _value.GetType().GetProperties())
                    {
                        Console.WriteLine($"{pi.Name}: {string.Join(',', pi.SetMethod!.ReturnParameter.GetRequiredCustomModifiers().Select(v => v.FullName))}");
                        Property prop = new PropertyInfoProperty(pi, _value);
                        Properties.Add(prop);
                    }
                }
            }
        }
    }
    public EditObject(string methodName, string parameterName)
    {
        _services = (IServiceProvider)Application.Current.Resources[ServiceProvider];
        _context = _services.GetRequiredService<PocotaContext>();
        Windows = _services.GetRequiredService<WindowsList>();
        MethodName = methodName;
        ParameterName = parameterName;
        InitializeComponent();
        CalcColumnsWidth(PropertiesView.ActualWidth);
        Windows.Touch();
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
        if(Launcher is { })
        {
            Launcher.Focus();
        }
    }
}
