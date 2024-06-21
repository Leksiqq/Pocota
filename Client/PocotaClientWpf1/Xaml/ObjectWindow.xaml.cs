using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Client.UserControls;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client;
public partial class ObjectWindow : Window, IServiceRelated, INotifyPropertyChanged, IEditWindow, IValueConverter
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private readonly INamesConverter _namesConverter;
    private object? _target;
    private string? _propertyName;
    private IInputElement? _currentInput = null;
    public string ServiceKey { get; private init; }
    public object? Target
    {
        get => _target;
        internal set
        {
            if (_target != value)
            {
                _target = value;
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
        }
    }
    public string? PropertyName
    {
        get => _propertyName;
        internal set
        {
            if (_propertyName != value)
            {
                _propertyName = value;
                PropertyChanged?.Invoke(this, _propertyChangedEventArgs);
            }
        }
    }
    public string ObjectTitle => GetType().FullName!;//$"{(Core.Launcher?.Owner is IEditWindow ew ? $"{ew.ObjectTitle}/" : string.Empty)}{ConvertName(PropertyName, Target?.GetType())}";
    public ObjectWindow(string serviceKey, Window owner)
    {
        _namesConverter = (Application.Current.Resources[ServiceProviderResourceKey] as IServiceProvider)!.GetRequiredService<INamesConverter>();
        ServiceKey = serviceKey;
        _mw = owner as MethodsWindow;
        InitializeComponent();
    }
    MethodsWindow? _mw;
    ~ObjectWindow()
    {
        Console.WriteLine();
        if(_mw is { })
        {
            Interlocked.Decrement(ref _mw.count);
        }
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
    private string? ConvertName(object? value, object? parameter = null)
    {
        return value is { } ? (string?)_namesConverter.Convert(value, typeof(string), parameter, CultureInfo.CurrentCulture) : string.Empty;
    }
    private void ObjectEditor_CurrentInputChanged(object sender, EventArgs e)
    {
        if (sender is ObjectEditor oe) 
        {
            InsertInputMode.Text = (string?)oe.Convert(oe.CurrentInput?.IsInsertMode, typeof(string), "InsertInputMode", CultureInfo.CurrentCulture);
        }
    }
}
