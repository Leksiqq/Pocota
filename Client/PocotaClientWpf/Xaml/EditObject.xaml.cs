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
    private readonly object _value;
    public ObservableCollection<NamedValue> Properties { get; private init; } = [];
    public bool IsReadonly { get; set; }
    public bool KeysOnly { get; set; }
    public EditObject(object value)
    {
        _services = (IServiceProvider)Application.Current.Resources[ServiceProvider];
        _context = _services.GetRequiredService<PocotaContext>();
        InitializeComponent();
        _value = value;
        if (PocotaContext.IsEntityType(_value.GetType()))
        {

        }
        else
        {
            foreach (PropertyInfo pi in _value.GetType().GetProperties())
            {
                NamedValue prop = new(pi.Name, pi.PropertyType)
                {
                    IsReadonly = !pi.CanWrite,
                    Value = pi.GetValue(_value),
                };
                Properties.Add(prop);
            }
        }
        CalcColumnsWidth(PropertiesView.ActualWidth);
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
}
