using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.Pocota.Client.UserControls;
using System.Globalization;
using System.Text.Json;
using System.Windows;
namespace Net.Leksi.Pocota.Client;
public partial class MethodWindow : Window
{
    private const string s_target = "target";
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(nameof(Target), typeof(object), typeof(MethodWindow));
    public static readonly DependencyProperty ServiceKeyProperty = DependencyProperty.Register(nameof(ServiceKey), typeof(string), typeof(MethodWindow));
    public static readonly DependencyProperty ReturnTypeProperty = DependencyProperty.Register(nameof(ReturnType), typeof(Type), typeof(MethodWindow));
    public static readonly DependencyProperty MethodNameProperty = DependencyProperty.Register(nameof(MethodName), typeof(string), typeof(MethodWindow));
    public static readonly DependencyProperty ObjectTitleProperty = DependencyProperty.Register(nameof(ObjectTitle), typeof(string), typeof(MethodWindow));
    private readonly INamesConverter _namesConverter = null!;
    private object? _target;
    public object? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }
    public Type ReturnType
    {
        get => (Type)GetValue(ReturnTypeProperty);
        set => SetValue(ReturnTypeProperty, value);
    }
    public string ObjectTitle
    {
        get => (string)GetValue(ObjectTitleProperty);
        set => SetValue(ObjectTitleProperty, value);
    }
    public string ServiceKey
    {
        get => (string)GetValue(ServiceKeyProperty);
        set => SetValue(ServiceKeyProperty, value);
    }
    public string MethodName
    {
        get => (string)GetValue(MethodNameProperty);
        set => SetValue(MethodNameProperty, value);
    }

    public MethodWindow()
    {
        _namesConverter = Application.Current.GetNamesConverter();
        Closed += Window1_Closed;
        InitializeComponent();
    }
    public void Init(ConnectorMethod connectorMethod)
    {
        if (connectorMethod != null)
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
        }
    }
    private string? ConvertName(object value, object? parameter = null)
    {
        return (string?)_namesConverter.Convert(value, typeof(string), parameter, CultureInfo.CurrentCulture);
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
    private void Window1_Closed(object? sender, EventArgs e)
    {
        ObjectEditor.Clear();
    }
}
