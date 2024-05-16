using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client;
public partial class EditList : Window, INotifyPropertyChanged, IEditWindow
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IServiceProvider _services;
    private readonly PocotaContext _context;
    private object? _value;
    private Window? _launchedBy;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    public WindowsList Windows { get; private init; }
    public Window? LaunchedBy
    {
        get => _launchedBy;
        set
        {
            if (_launchedBy != value)
            {
                _launchedBy = value;
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
        }
    }
    public EditWindowCore EditWindowCore { get; private init; }
    public object? Value
    {
        get => _value;
        set
        {
            if (_value != value && value is { })
            {
                _value = value;
            }
        }
    }
    public EditList(string path, Type type)
    {
        _services = (IServiceProvider)Application.Current.Resources[ServiceProvider];
        _context = _services.GetRequiredService<PocotaContext>();
        Windows = _services.GetRequiredService<WindowsList>();
        EditWindowCore = new EditWindowCore(path, type);
        InitializeComponent();
        Windows.Touch();
    }
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (LaunchedBy is { })
        {
            LaunchedBy.Focus();
        }
    }
}
