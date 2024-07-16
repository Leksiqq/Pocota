using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Client.UserControls;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
namespace Net.Leksi.Pocota.Client;
public partial class MethodWindow : Window, IServiceRelated, IEditWindow, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private const string s_target = "target";
    private readonly INamesConverter _namesConverter = null!;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private ConnectorMethod _connectorMethod = null!;
    private object? _target;
    public object? Target => _target;
    public string? MethodName => _connectorMethod?.Method.Name;
    public string? ServiceKey => _connectorMethod?.ServiceKey;
    public Type? ReturnType => _connectorMethod != null ? 
        _connectorMethod.Method.GetParameters()
            .Where(p => p.Name == s_target).FirstOrDefault()?.ParameterType 
                ?? _connectorMethod.Method.ReturnType.GetGenericArguments()[0]
        : null;

    public string ObjectTitle => _connectorMethod != null ?
        $"{ConvertName(ServiceKey!)}:{ConvertName(_connectorMethod.Method.Name, _connectorMethod.Connector)}()" : string.Empty;
    public MethodWindow()
    {
        _namesConverter = Application.Current.GetNamesConverter();
    }
    public void Init(ConnectorMethod connectorMethod) 
    {
        //_connectorMethod = connectorMethod;
        //Type targetType = Application.Current.GetServiceProvider()
        //        .GetRequiredKeyedService<Connector>(ServiceKey)
        //        .GetMethodOptionsType(_connectorMethod.Method)!;
        //if (targetType != null)
        //{
        //    _target = Activator.CreateInstance(targetType);
        //}
        InitializeComponent();
    }
    private string? ConvertName(object value, object? parameter = null)
    {
        return (string?)_namesConverter.Convert(value, typeof(string), parameter, CultureInfo.CurrentCulture);
    }
    protected override void OnActivated(EventArgs e)
    {
        ObjectEditor.CalcColumnsWidth();
        base.OnActivated(e);
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine(JsonSerializer.Serialize(Target));
    }
    private void ObjectEditor_CurrentInputChanged(object sender, EventArgs e)
    {
        if (sender is ObjectEditor oe)
        {
            InsertInputMode.Text = (string?)oe.Convert(oe.CurrentInput?.IsInsertMode, typeof(string), "InsertInputMode", CultureInfo.CurrentCulture);
        }
    }
}
