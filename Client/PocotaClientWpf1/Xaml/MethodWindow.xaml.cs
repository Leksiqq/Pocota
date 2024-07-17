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
    private object? _target;
    public object? Target { get; private set; }
    public Type ReturnType { get; private set; } = null!;
    public string ObjectTitle { get; private set; } = null!;
    public string ServiceKey { get; private set; } = null!;
    public string MethodName { get; private set; } = null!;
    public MethodWindow()
    {
        _namesConverter = Application.Current.GetNamesConverter();
    }
    public void Init(ConnectorMethod connectorMethod) 
    {
        ServiceKey = connectorMethod.ServiceKey;
        MethodName = connectorMethod.Method.Name;
        ObjectTitle = $"{ConvertName(ServiceKey!)}:{ConvertName(connectorMethod.Method.Name, connectorMethod.Connector)}()";
        ReturnType = connectorMethod.Method.GetParameters()
            .Where(p => p.Name == s_target).FirstOrDefault()?.ParameterType
                ?? connectorMethod.Method.ReturnType.GetGenericArguments()[0];
        Type targetType = Application.Current.GetServiceProvider()
                .GetRequiredKeyedService<Connector>(ServiceKey)
                .GetMethodOptionsType(connectorMethod.Method)!;
        if (targetType != null)
        {
            Target = Activator.CreateInstance(targetType)!;
        }
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
