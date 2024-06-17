using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client;
public partial class MethodWindow : Window, IWindowWithCore, IServiceRelated, IEditWindow, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private const string s_target = "target";
    private readonly ConnectorMethod _connectorMethod;
    private readonly INamesConverter _namesConverter;
    public ObservableCollection<Property> Parameters { get; private init; } = [];
    public WindowCore Core { get; private init; }
    public string MethodName => _connectorMethod.Method.Name;
    public string ServiceKey => _connectorMethod.ServiceKey;
    public Type ReturnType => _connectorMethod.Method.GetParameters()
        .Where(p => p.Name == s_target).FirstOrDefault()?.ParameterType ?? _connectorMethod.Method.ReturnType.GetGenericArguments()[0];

    public string ObjectTitle => $"{ConvertName(ServiceKey)}:{ConvertName(_connectorMethod.Method.Name, _connectorMethod.Connector)}()";
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    private MethodWindow()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    {
        _namesConverter = (Application.Current.Resources[ServiceProviderResourceKey] as IServiceProvider)!.GetRequiredService<INamesConverter>();
        Core = new WindowCore(this);
    }
    public MethodWindow(Delegate @delegate) : this()
    {
        _connectorMethod = GetConnectorMethod(@delegate);
        Init();
    }

    private ConnectorMethod GetConnectorMethod(Delegate @delegate)
    {
        return (Application.Current.Resources[ServiceProviderResourceKey] as IServiceProvider)!.GetRequiredService<ConnectorsMethodsList>()[@delegate.Method]!;
    }

    public MethodWindow(ConnectorMethod connectorMethod): this()
    {
        _connectorMethod = connectorMethod;
        Init();
    }
    private string? ConvertName(object value, object? parameter = null)
    {
        return (string?)_namesConverter.Convert(value, typeof(string), parameter, CultureInfo.CurrentCulture);
    }
    private void Init()
    {
        InitializeComponent();
        foreach (ParameterInfo parameter in _connectorMethod.Method.GetParameters())
        {
            if (parameter.ParameterType != typeof(CancellationToken) && parameter.Name != s_target)
            {
                Parameters.Add(Property.Create(parameter)!);
            }
        }
    }

    protected override void OnActivated(EventArgs e)
    {
        ObjectEditor.CalcColumnsWidth();
        base.OnActivated(e);
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine(JsonSerializer.Serialize(Parameters.Select(p => p.Value)));
    }

    private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi)
        {
            foreach (var item in mi.Items)
            {
            }
        }
    }
}
