using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Client.UserControls;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace Net.Leksi.Pocota.Client;
public partial class ObjectWindow : Window, IConnectorNameRelated, INotifyPropertyChanged, IEditWindow, IValueConverter
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private readonly INamesConverter _namesConverter;
    private object? _target;
    private string? _propertyName;
    private IInputElement? _currentInput = null;
    public string ConnectorName { get; private set; } = string.Empty;
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
    public string ObjectTitle => $"{(Application.Current.GetApplicationCore().GetLauncher(this) is IEditWindow ew 
        ? $"{ew.ObjectTitle}/" 
        : string.Empty)}{ConvertName(PropertyName, Target?.GetType())}"
    ;
    public ObjectWindow()
    {
        _namesConverter = Application.Current.GetServiceProvider().GetRequiredService<INamesConverter>();
        Closed += ObjectWindow_Closed;
        InitializeComponent();
    }

    private void ObjectWindow_Closed(object? sender, EventArgs e)
    {
        ObjectEditor.Clear();
        Target = null;
    }

    public void Init(string connectorName)
    {
        ConnectorName = connectorName;
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
        return value != null ? (string?)_namesConverter.Convert(value, typeof(string), parameter, CultureInfo.CurrentCulture) : string.Empty;
    }
    private void ObjectEditor_CurrentInputChanged(object sender, EventArgs e)
    {
        if (sender is ObjectEditor oe) 
        {
            InsertInputMode.Text = (string?)oe.Convert(oe.CurrentInput?.IsInsertMode, typeof(string), "InsertInputMode", CultureInfo.CurrentCulture);
        }
    }
}
