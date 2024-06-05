using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client.UserControls;
public partial class ObjectField : UserControl, ICommand, IValueConverter, IServiceRelated
{
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }

    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
       nameof(Property), typeof(Property),
       typeof(ObjectField)
    );
    public static readonly DependencyProperty WindowProperty = DependencyProperty.Register(
       nameof(Window), typeof(Window),
       typeof(ObjectField)
    );
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private string _serviceKey = string.Empty;
    public Property Property
    {
        get => (Property)GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }
    public Window Window
    {
        get => (Window)GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }
    public string ServiceKey => _serviceKey;
    public ObjectField()
    {
        InitializeComponent();
    }
    public bool CanExecute(object? parameter)
    {
        return 
            Property is { } && (
                (Property.Value is { } && "Edit".Equals(parameter))
                || (!Property.IsReadonly && ("Find".Equals(parameter) || "Create".Equals(parameter)))
                || (!Property.IsReadonly && Property.Value is { } && "Clear".Equals(parameter))
            )
            ;
    }
    public void Execute(object? parameter)
    {
        if(Property is { })
        {
            if(!Property.IsReadonly && "Create".Equals(parameter))
            {
                Property.Value = ((IServiceProvider)FindResource(ServiceProviderResourceKey))
                    .GetRequiredKeyedService<PocotaContext>(ServiceKey).CreateInstance(Property.Type);
            }
            else if(!Property.IsReadonly && Property.Value is { } && "Clear".Equals(parameter))
            {
                Property.Value = default;
            }
        }
    }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(Property is {})
        {
            if ("ObjectState".Equals(parameter))
            {
                if (Property.Access is Contract.AccessKind.NotSet)
                {
                    return ObjectState.NotSet;
                }
                if (Property.Access is Contract.AccessKind.Forbidden)
                {
                    return ObjectState.Forbidden;
                }
                if (Property.Value is { })
                {  
                    return ObjectState.IsNotNull;
                }
                return ObjectState.IsNull;
            }
        }
        return value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == PropertyProperty)
        {
            if (e.OldValue is Property oldProperty)
            {
                TextBlock.DataContext = null;
            }
            if (e.NewValue is Property newProperty)
            {
                TextBlock.DataContext = newProperty;
            }
        }
        else if (e.Property == WindowProperty)
        {
            if (e.OldValue is Window)
            {
                _serviceKey = string.Empty;
            }
            if (e.NewValue is Window)
            {
                if (e.NewValue is IServiceRelated sr)
                {
                    _serviceKey = sr.ServiceKey;
                }
            }
        }
        base.OnPropertyChanged(e);
    }
}
