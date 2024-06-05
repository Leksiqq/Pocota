using Net.Leksi.WpfMarkup;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client.UserControls;

public partial class ObjectEditor : UserControl
{
    public static readonly DependencyProperty ServiceProviderCatcherProperty = DependencyProperty.Register(
       nameof(ServiceProviderCatcher), typeof(XamlServiceProviderCatcher),
       typeof(ObjectEditor)
    );
    public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(
       nameof(Properties), typeof(ObservableCollection<Property>),
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
            if (e.OldValue is { })
            {
                PropertiesViewSource.Source = null;
            }
            if (e.NewValue is ObservableCollection<Property> oc)
            {
                Console.WriteLine(oc.Count);
                PropertiesViewSource.Source = Properties;
            }
        }
        else if (e.Property == ServiceProviderCatcherProperty)
        {
            Console.WriteLine(e.NewValue);
        }
        base.OnPropertyChanged(e);
    }
}
