using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
namespace Net.Leksi.Pocota.Client;
public partial class MethodWindow : Window, IWindowWithCore, IServiceRelated
{
    private const string s_target = "target";
    private readonly ConnectorMethod _connectorMethod;
    public ObservableCollection<Property> Parameters { get; private init; } = [];
    public WindowCore Core { get; private init; }
    public string MethodName => _connectorMethod.Method.Name;
    public string ServiceKey => _connectorMethod.ServiceKey;
    public Type ReturnType => _connectorMethod.Method.GetParameters()
        .Where(p => p.Name == s_target).FirstOrDefault()?.ParameterType ?? _connectorMethod.Method.ReturnType.GetGenericArguments()[0];
    public MethodWindow(ConnectorMethod connectorMethod)
    {
        _connectorMethod = connectorMethod;
        Core = new WindowCore(this);
        foreach (ParameterInfo parameter in _connectorMethod.Method.GetParameters())
        {
            if (parameter.ParameterType != typeof(CancellationToken) && parameter.Name != s_target)
            {
                Parameters.Add(Property.Create(parameter)!);
            }
        }
        InitializeComponent();
        //CalcColumnsWidth(ParametersView.ActualWidth);
    }
    //private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
    //{
    //    if (e.WidthChanged)
    //    {
    //        CalcColumnsWidth(e.NewSize.Width);
    //    }
    //}
    //private void CalcColumnsWidth(double width)
    //{
    //    ParameterValueColumn.Width = width * 0.89 - ParameterNameColumn.ActualWidth;
    //}
    //private void ListViewItem_SizeChanged(object sender, SizeChangedEventArgs e)
    //{
    //    CalcColumnsWidth(ParametersView.ActualWidth);
    //}
}
