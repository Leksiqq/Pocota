using Net.Leksi.WpfMarkup;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client.UserControls;

public partial class ObjectEditor : UserControl, IValueConverter
{
    public static readonly DependencyProperty ServiceProviderCatcherProperty = DependencyProperty.Register(
       nameof(ServiceProviderCatcher), typeof(XamlServiceProviderCatcher),
       typeof(ObjectEditor)
    );
    public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(
       nameof(Properties), typeof(ObservableCollection<Property>),
       typeof(ObjectEditor)
    );
    public static readonly DependencyProperty WindowProperty = DependencyProperty.Register(
       nameof(Window), typeof(Window),
       typeof(ObjectEditor)
    );
    public CollectionViewSource PropertiesViewSource { get; private init; } = new();
    public XamlServiceProviderCatcher ServiceProviderCatcher 
    { 
        get => (XamlServiceProviderCatcher)GetValue(ServiceProviderCatcherProperty); 
        set => SetValue(ServiceProviderCatcherProperty, value);
    }
    public ObservableCollection<Property> Properties
    {
        get => (ObservableCollection<Property>)GetValue(PropertiesProperty);
        set => SetValue(PropertiesProperty, value);
    }
    public Window Window
    {
        get => (Window)GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }
    public ObjectEditor()
    {
        InitializeComponent();
    }
    private void CalcColumnsWidth(double width)
    {
        PropertyValueColumn.Width = width * 0.8 - PropertyNameColumn.ActualWidth;
    }
    private void PropertiesView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            CalcColumnsWidth(PropertiesView.ActualWidth);
        }
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == PropertiesProperty)
        {
            Console.WriteLine($"{e.Property}: {e.NewValue}");
            SetTemplateSelector();
        }
        else if (e.Property == ServiceProviderCatcherProperty)
        {
            Console.WriteLine($"{e.Property}: {e.NewValue}");
            SetTemplateSelector();
        }
        else if (e.Property == WindowProperty)
        {
            Console.WriteLine($"{e.Property}: {e.NewValue}");
            SetTemplateSelector();
        }
        base.OnPropertyChanged(e);
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine($"Convert: {value}");
        return value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine($"ConvertBack: {value}");
        return value;
    }
    private void SetTemplateSelector()
    {
        if (ServiceProviderCatcher is { } && Properties is { } && Window is { } && PropertyValueColumn.CellTemplateSelector is null)
        {
            string spName = $"sp{Guid.NewGuid}";
            ParameterizedResourceExtension pre = new("PropertyTemplateSelector")
            {
                Replaces = new string[] { $"$serviceProviderCatcher:{spName}" },
                Verbose = 2,
            };
            this.Window.Resources.Add(spName, ServiceProviderCatcher);
            PropertyValueColumn.CellTemplateSelector = pre.ProvideValue(ServiceProviderCatcher.ServiceProvider!) as DataTemplateSelector;
            this.Window.Resources.Remove(spName);
            PropertiesViewSource.Source = Properties;
        }
    }
}
