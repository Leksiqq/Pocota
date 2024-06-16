using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
namespace Net.Leksi.Pocota.Client;
public partial class MethodWindow : Window, IWindowWithCore, IServiceRelated, IEditWindow
{
    private const string s_target = "target";
    private readonly ConnectorMethod _connectorMethod;
    public ObservableCollection<Property> Parameters { get; private init; } = [];
    public WindowCore Core { get; private init; }
    public string MethodName => _connectorMethod.Method.Name;
    public string ServiceKey => _connectorMethod.ServiceKey;
    public Type ReturnType => _connectorMethod.Method.GetParameters()
        .Where(p => p.Name == s_target).FirstOrDefault()?.ParameterType ?? _connectorMethod.Method.ReturnType.GetGenericArguments()[0];

    public string ObjectTitle => $"{ServiceKey}:{_connectorMethod.Method.Name}()";

    public MethodWindow(ConnectorMethod connectorMethod)
    {
        _connectorMethod = connectorMethod;
        Core = new WindowCore(this);
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
