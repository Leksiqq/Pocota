using Net.Leksi.WpfMarkup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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
    private void CalcColumnsWidth()
    {
        PropertyValueColumn.Width = PropertiesView.ActualWidth - PropertyNameColumn.ActualWidth - 10;
        ScrollViewer scrollViewer = GetDescendants(Window).OfType<ScrollViewer>().First();
        if(scrollViewer.ComputedHorizontalScrollBarVisibility is Visibility.Visible)
        {
            Window.Width = Window.ActualWidth + 1;
        }
    }
    private void PropertiesView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            CalcColumnsWidth();
        }
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {

        if (e.Property.OwnerType == GetType())
        {
            if(e.Property == PropertiesProperty)
            {
                if (e.OldValue is ObservableCollection<Property> oc)
                {
                    oc.CollectionChanged -= Properties_CollectionChanged;
                }
                if (e.NewValue is ObservableCollection<Property> oc1)
                {
                    oc1.CollectionChanged += Properties_CollectionChanged;
                }
            }
            SetTemplateSelector();
        }
        base.OnPropertyChanged(e);
    }

    private void Properties_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CalcColumnsWidth();
    }

    private void SetTemplateSelector()
    {
        if (ServiceProviderCatcher is { } && Properties is { } && Window is { } && PropertyValueColumn.CellTemplateSelector is null)
        {
            string spName = $"sp{Guid.NewGuid()}";
            ParameterizedResourceExtension pre = new("PropertyTemplateSelector")
            {
                Replaces = new string[] { $"$serviceProviderCatcher:{spName}" },
            };
            this.Window.Resources.Add(spName, ServiceProviderCatcher);
            PropertyValueColumn.CellTemplateSelector = pre.ProvideValue(ServiceProviderCatcher.ServiceProvider!) as DataTemplateSelector;
            this.Window.Resources.Remove(spName);
            PropertiesViewSource.Source = Properties;
        }
    }

    private void oe_Loaded(object sender, RoutedEventArgs e)
    {
        CalcColumnsWidth();
    }
    private static IEnumerable<DependencyObject> GetDescendants(DependencyObject obj)
    {
        int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
        for(int i = 0; i < childrenCount; ++i)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            yield return child;
            foreach (var descendant in GetDescendants(child))
            {
                yield return descendant;
            }
        }
    }
}
